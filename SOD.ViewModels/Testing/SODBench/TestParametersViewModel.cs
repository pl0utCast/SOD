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
using SOD.Core.Sensor.TenzoSensor;
using SOD.Core;
using SOD.App.Testing.Programms;
using SOD.ViewModels.Testing.ManualCommandsBench;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Helpers;
using SOD.App.Commands;
using SOD.App.Messages.Commands;
using System.Threading;

namespace SOD.ViewModels.Testing.SODBench
{
	public class TestParametersViewModel : ReactiveValidationObject, IActivatableViewModel
	{
		private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
        private IBus _bus;

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
			_bus = bus;
			
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
			TenzoSensors.Add(sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable && s is ITenzoSensor));
			TenzoSensor = TenzoSensors.FirstOrDefault(x => x.Id == testSettings.TenzoSensorId);

			TenzoUnits = new Force().GetUnitTypeInfo();
			SelectedTenzoUnit = TenzoUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.TenzoUnit));

			ProgrammMethodics = new SelectProgrammMethodicsViewModel(bus, testingService, navigationService, /*valveService,*/
				dialogService, testBenchService, localizationService);
			ProgrammMethodics.Activator.Activate();

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

			var canApply = this.WhenAny(x => x.SelectedStandart, (selectedSt) => selectedSt != null);

			Apply = ReactiveCommand.CreateFromTask(async () =>
			{
				bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
				bench.Settings.TenzoUnit = (ForceUnit)SelectedTenzoUnit?.UnitType;
				bench.Settings.SelectedBalloon = SelectedBalloon;
				bench.Settings.SelectedBalloon.StandartId = SelectedStandart.Id;
				bench.Settings.SelectedBalloon.BalloonVolume = BalloonVolume;
				bench.Settings.BalloonProperties = BalloonProperties.ToList();

				if (!IsModeAuto)
				{
					testSettings.TenzoSensorId = TenzoSensor.Id;
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

            ExecuteCommand = ReactiveCommand.Create(() =>
            {
				var command = CommandsHelper.GetDefault(CommandCollectionType.Modbus3Post, SelectedCommand);
                _bus.Publish(new ExecuteTestCommand(command, true));
            });
        }

		public IEnumerable<IValueViewModel> Properties => parameters.Select(kv => kv.Value);
		public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedPressureUnit { get; set; }
		public IReadOnlyList<UnitTypeInfo> TenzoUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedTenzoUnit { get; set; }
		[Reactive]
		public Balloon SelectedBalloon { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }
		public ReactiveCommand<Unit, Unit> Apply { get; set; }
		public ReactiveCommand<Unit, Unit> ExecuteCommand { get; set; }
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
		public ObservableCollection<ISensor> TenzoSensors {  get; set; } = new ObservableCollection<ISensor> { };
		[Reactive]
		public ISensor TenzoSensor {  get; set; }
        [Reactive]
        public SelectProgrammMethodicsViewModel ProgrammMethodics { get; set; }
        [Reactive]
        public CommandType SelectedCommand { get; set; }
    }
}
