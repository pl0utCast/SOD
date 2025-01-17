using DynamicData;
using DynamicData.Binding;
using MemBus;
using SOD.Core.Infrastructure;
using SOD.Core.Units;
using SOD.App.Benches;
using SOD.App.Testing;
using SOD.ViewModels.Controls;
using SOD.ViewModels.Extensions;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;
using UnitsNet;
using UnitsNet.Units;
using SOD.App.Testing.Standarts;
using SOD.Core.Sensor;
using System.Collections.ObjectModel;
using SOD.Core.Balloons;
using SOD.Core.Balloons.Properties;
using SOD.Core.Sensor.TensoSensor;
using SOD.Core;

namespace SOD.ViewModels.Testing.SODBench
{
	public class TestParametersViewModel : ReactiveObject, IActivatableViewModel
	{
		private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
		public TestParametersViewModel(INavigationService navigationService,
									   ITestBenchService testBenchService,
									   ITestingService testingService,
									   ISensorService sensorService,
									   ILocalizationService localizationService,
									   IBus bus,
									   IDialogService dialogService,
									   IReportService reportService)
		{
			var bench = (App.Benches.SODBench.Bench)testBenchService.GetTestBench();
			var testSettings = bench.Settings.SelectedTestSettings;
			
			Standarts = testingService.GetAllStandarts().ToList();
			SelectedStandart = Standarts.SingleOrDefault(u => u.Id == bench.Settings.SelectedBalloon?.StandartId);

			PressureUnits = new Pressure().GetUnitTypeInfo();
			SelectedPressureUnit = PressureUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.PressureUnit));
			WorkPressure = new UnitValueViewModel(bench.Settings.SelectedTestSettings.SetPressure);

			foreach (BalloonTypes balloon in Enum.GetValues(typeof(BalloonTypes)))
				Balloons.Add(new Balloon() { BalloonTypes = balloon, Name = localizationService["Prefixes." + balloon.ToString()] });

			SelectedBalloon = Balloons.FirstOrDefault(x => x.BalloonTypes == bench.Settings.SelectedBalloon.BalloonTypes);
			BalloonVolume = bench.Settings.SelectedBalloon.BalloonVolume;

			ExposureTime = testSettings.Time;
			MaxDeformation = testSettings.MaxDeformation.ToString();
			
			PressureSensors.Add(sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable && s is IPressureSensor));
			PressureSensor = PressureSensors.FirstOrDefault(x => x.Id == testSettings.PressureSensorId);
			
			IsModeAuto = testSettings.IsModeAuto;
			TensoSensors.Add(sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable && s is ITensoSensor));
			TensoSensor = TensoSensors.FirstOrDefault(x => x.Id == testSettings.TensoSensorId);

			TensoUnits = new Mass().GetUnitTypeInfo();
			SelectedTensoUnit = TensoUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.TensoUnit));

			this.WhenAnyValue(x => x.SelectedBalloon).Subscribe(sb =>
			{
				IsKPG4 = sb?.BalloonTypes == BalloonTypes.KPG4;
				Deformation = IsKPG4 ? 10 : 5;
			});

			this.WhenAnyValue(x => x.MaxDeformation).Subscribe(md =>
			{
				var t = int.TryParse(md, out var value);
				if (!t)
					MaxDeformation = string.Empty;
			});

			foreach (var balloonProp in bench.Settings.BalloonProperties)
			{
				BalloonProperties.Add(balloonProp);
			}

			foreach (var param in bench.Settings.Parameters)
			{
				parameters.Add(param.Alias, param.GetValueViewModel());
			}

			Cancel = ReactiveCommand.Create(() =>
			{
				navigationService.GoBack();
			});

			IObservable<bool> canApply = this.WhenAnyValue(
				x => x.SelectedStandart, x => x.IsConfirmed,
				(selectedSt, isConf) => selectedSt != null && isConf);

			Apply = ReactiveCommand.CreateFromTask(async () =>
			{
				bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
				bench.Settings.TensoUnit = (MassUnit)SelectedTensoUnit?.UnitType;
				bench.Settings.SelectedBalloon = SelectedBalloon;
				bench.Settings.SelectedBalloon.StandartId = SelectedStandart.Id;
				bench.Settings.SelectedBalloon.BalloonVolume = BalloonVolume;
				bench.Settings.BalloonProperties = BalloonProperties.ToList();

				if (!IsModeAuto)
				{
					testSettings.TensoSensorId = TensoSensor.Id;
				}
				testSettings.SetPressure = (Pressure)UnitsHelper.GetValue(WorkPressure.Value, WorkPressure.SelectedUnitInfo);
				testSettings.Deformation = Deformation;
				var t = int.TryParse(MaxDeformation, out var value);
				testSettings.MaxDeformation = t ? value : null;
				testSettings.IsModeAuto = IsModeAuto;
				testSettings.Time = ExposureTime;
				testSettings.PressureSensorId = PressureSensor?.Id;
				

				if (reportService.CurrentReport != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill)
				{
					var result = await dialogService.ShowDialogAsync("CreateNewReport", new YesNoDialogViewModel(dialogService));
					if ((bool)result)
					{
						reportService.Save(reportService.CurrentReport);
						bench.UpdateReport();
					}
				}
				bench.TestingBalloon = SelectedBalloon;
				bench.UpdatePosts();

				foreach (var param in parameters)
				{
					var parameter = bench.Settings.Parameters.SingleOrDefault(p => p.Alias == param.Key);
					if (parameter != null)
					{
						parameter.Value = param.Value.GetValue();
					}
				}

				bench.SaveSettings();

				bus.Publish(new App.Benches.SODBench.Messages.SelectedTestMessage());

				navigationService.GoBack();
			}, canApply);

		}

		public IEnumerable<IValueViewModel> Properties => parameters.Select(kv => kv.Value);
		public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedPressureUnit { get; set; }
		public IReadOnlyList<UnitTypeInfo> TensoUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedTensoUnit { get; set; }
		[Reactive]
		public Balloon SelectedBalloon { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }
		public ReactiveCommand<Unit, Unit> Apply { get; set; }
		public ViewModelActivator Activator { get; } = new ViewModelActivator();
		[Reactive]
		public List<Balloon> Balloons { get; set; } = new List<Balloon>();
		[Reactive]
		public IStandart SelectedStandart { get; set; }
		public IEnumerable<IStandart> Standarts { get; set; }
		[Reactive]
		public double BalloonVolume { get; set; }
		public ObservableCollection<BalloonProperty> BalloonProperties { get; set; } = new ObservableCollection<BalloonProperty>();
		[Reactive]
		public UnitValueViewModel WorkPressure { get; set; }
		[Reactive]
		public int Deformation { get; set; }
		[Reactive]
		public int? ExposureTime { get; set; }
		[Reactive]
		public string MaxDeformation { get; set; }
		[Reactive]
		public bool IsKPG4 { get; set; }
		[Reactive]
		public bool IsModeAuto { get; set; }
		[Reactive]
		public bool IsConfirmed { get; set; }
		public ObservableCollection<ISensor> PressureSensors { get; set; } = new ObservableCollection<ISensor>();
		[Reactive]
		public ISensor PressureSensor { get; set; }
		public ObservableCollection<ISensor> TensoSensors {  get; set; } = new ObservableCollection<ISensor> { };
		[Reactive]
		public ISensor TensoSensor {  get; set; }
	}
}
