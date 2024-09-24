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

namespace SOD.ViewModels.Testing.CRSBench
{
	public class TestParametersViewModel : ReactiveObject, IActivatableViewModel
	{
		private SourceList<Tuple<int, TestSettingsViewModel>> tests { get; set; } = new SourceList<Tuple<int, TestSettingsViewModel>>();
		private ObservableAsPropertyHelper<bool> isSelectedTest;
		private ObservableAsPropertyHelper<bool> isKPG4;
		private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
		public TestParametersViewModel(INavigationService navigationService,
									   IValveService valveService,
									   ITestBenchService testBenchService,
									   ITestingService testingService,
									   ISensorService sensorService,
									   ILocalizationService localizationService,
									   IBus bus,
									   IDialogService dialogService,
									   IReportService reportService)
		{

			var bench = (App.Benches.CRSBench.Bench)testBenchService.GetTestBench();
			var standarts = testingService.GetAllStandarts().ToList();
			//standarts.Add(new LuaStandart() { Id = 0, Name = "не выбран" });
			PressureUnits = new Pressure().GetUnitTypeInfo();
			SelectedPressureUnit = PressureUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.PressureUnit));
			//LeakageUnits = new VolumeFlow().GetUnitTypeInfo();
			//SelectedLeakageUnit = LeakageUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.LeakageUnit));

			foreach (BalloonType balloon in Enum.GetValues(typeof(BalloonType)))
				Balloons.Add(new Balloon() { BalloonType = balloon, Name=balloon.ToString()});

			SelectedBalloon = Balloons.FirstOrDefault(x=>x.BalloonType == bench.Settings.SelectedTestSettings.BalloonType);
			SelectedStandart = standarts.SingleOrDefault(u => u.Id == bench.Settings.SelectedTestSettings.StandartId);
			BalloonValue = bench.Settings.BalloonValue;
			Standarts = standarts;
			BalloonSN = bench.Settings.BalloonSN;
			ExposureTime = bench.Settings.SelectedTestSettings.Time;
			WorkPressure = bench.Settings.SelectedTestSettings.WorkPressure;
			IsModeAuto = bench.Settings.SelectedTestSettings.IsModeAuto;
			MaxDeformation = bench.Settings.SelectedTestSettings.MaxDeformation.ToString();
			var testSet = bench.Settings.SelectedTestSettings;
			PressureSensors.Add( sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable && s is IPressureSensor));
			PressureSensor = PressureSensors.FirstOrDefault(x => x.Id == testSet.PressureSensorId);

			this.WhenAnyValue(x => x.SelectedBalloon).Subscribe(x =>
			{
				IsKPG4 = x?.BalloonType == BalloonType.KPG4;
				Deformation = IsKPG4 ? 10 : 5;
				//BalloonSN = null;
				//SelectedStandart = null;
				//BalloonValue = null;
			});

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
				bench.Settings.SelectedTestSettings.BalloonType = SelectedBalloon.BalloonType;
				bench.Settings.SelectedTestSettings.WorkPressure = WorkPressure;
				bench.Settings.SelectedTestSettings.Deformation = Deformation;
				var t = int.TryParse(MaxDeformation, out var value);
				bench.Settings.SelectedTestSettings.MaxDeformation = t ? value : null;
				bench.Settings.SelectedTestSettings.IsModeAuto = IsModeAuto;
				bench.Settings.BalloonValue = (int)BalloonValue;
				bench.Settings.BalloonSN = BalloonSN;
				bench.Settings.SelectedTestSettings.Time = ExposureTime;
				bench.Settings.SelectedTestSettings.StandartId = SelectedStandart?.Id;
				bench.Settings.SelectedTestSettings.PressureSensorId = PressureSensor?.Id;

				if (reportService.CurrentReport != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill)
				{
					var result = await dialogService.ShowDialogAsync("CreateNewReport", new YesNoDialogViewModel(dialogService));
					if ((bool)result)
					{
						reportService.Save(reportService.CurrentReport);
						bench.UpdateReport();
					}
				}

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

					bus.Publish(new App.Benches.CRSBench.Messages.SelectedTestMessage());

				navigationService.GoBack();
			}, canApply);

		}

		public IEnumerable<IValueViewModel> Properties => parameters.Select(kv => kv.Value);
		public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedPressureUnit { get; set; }
		//public IReadOnlyList<UnitTypeInfo> LeakageUnits { get; set; }
		//[Reactive]
		//public UnitTypeInfo SelectedLeakageUnit { get; set; }
		public ObservableCollectionExtended<TestSettingsViewModel> Tests { get; set; } = new ObservableCollectionExtended<TestSettingsViewModel>();
		[Reactive]
		public TestSettingsViewModel SelectedTest { get; set; }
		[Reactive]
		public Balloon SelectedBalloon { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }
		public ReactiveCommand<Unit, Unit> Apply { get; set; }
		public bool IsSelectedTest => isSelectedTest.Value;
		public ViewModelActivator Activator { get; } = new ViewModelActivator();
		[Reactive]
		public List<Balloon> Balloons { get; set; } = new List<Balloon>();
		[Reactive]
		public IStandart SelectedStandart { get; set; }
		public IEnumerable<IStandart> Standarts { get; set; }
		[Reactive]
		public int? BalloonValue { get; set; }
		[Reactive]
		public string BalloonSN { get; set; }
		[Reactive]
		public int WorkPressure { get; set; }
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
		public class Balloon
		{
			public BalloonType BalloonType { get; set; }
			public string Name { get; set; }
		}
	}
}
