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
using SOD.Core.Device.Modbus;
using System.Windows.Media.Media3D;
using SOD.Core.Device;
using SOD.App.Benches.SODBench;
using SOD.Core.Props;
using SOD.Localization.Testing.SODBench;

namespace SOD.ViewModels.Testing.SODBench
{
	public class TestParametersViewModel : ReactiveObject, IActivatableViewModel
	{
        private IDevice device;
        private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
		private TestBenchSettings serviceParameters = new TestBenchSettings();
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
			TenzoSensors.Add(sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable && s is ITenzoSensor));
			TenzoSensor = TenzoSensors.FirstOrDefault(x => x.Id == testSettings.TenzoSensorId);

			TenzoUnits = new Force().GetUnitTypeInfo();
			SelectedTenzoUnit = TenzoUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.TenzoUnit));

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
			//Чтение сервисных параметров
			serviceParameters.PressureVelocity = bench.Settings.TestBenchSettings.PressureVelocity;
            serviceParameters.HydraulicPressureCoef = bench.Settings.TestBenchSettings.HydraulicPressureCoef;
            serviceParameters.MaxAllowablePressure = bench.Settings.TestBenchSettings.MaxAllowablePressure;
            serviceParameters.ThrottleActivationPercentage = bench.Settings.TestBenchSettings.ThrottleActivationPercentage;
            serviceParameters.ThrottleDeactivationPercentage = bench.Settings.TestBenchSettings.ThrottleDeactivationPercentage;
            serviceParameters.OverpressureAllowancePercentage = bench.Settings.TestBenchSettings.OverpressureAllowancePercentage;
            serviceParameters.NominalPressureValue = bench.Settings.TestBenchSettings.NominalPressureValue;
            serviceParameters.Reserve1 = bench.Settings.TestBenchSettings.Reserve1;
            serviceParameters.Reserve2 = bench.Settings.TestBenchSettings.Reserve2;
            serviceParameters.VesselEmergencyLevel_5kg = bench.Settings.TestBenchSettings.VesselEmergencyLevel_5kg;
            serviceParameters.VesselEmergencyLevel_10kg = bench.Settings.TestBenchSettings.VesselEmergencyLevel_10kg;
            serviceParameters.VesselEmergencyLevel_30kg = bench.Settings.TestBenchSettings.VesselEmergencyLevel_30kg;
            serviceParameters.KpPID = bench.Settings.TestBenchSettings.KpPID;
            serviceParameters.KiPID = bench.Settings.TestBenchSettings.KiPID;
            serviceParameters.KdPID = bench.Settings.TestBenchSettings.KdPID;
            serviceParameters.dwePID = bench.Settings.TestBenchSettings.dwePID;
            serviceParameters.tfPID = bench.Settings.TestBenchSettings.tfPID;
            serviceParameters.MaxOutputConst = bench.Settings.TestBenchSettings.MaxOutputConst;
            serviceParameters.ErrorZonePID = bench.Settings.TestBenchSettings.ErrorZonePID;
            serviceParameters.MinOutputConst = bench.Settings.TestBenchSettings.MinOutputConst;
            serviceParameters.CyclePID = bench.Settings.TestBenchSettings.CyclePID;

            Cancel = ReactiveCommand.Create(() =>
			{
				navigationService.GoBack();
			});
#if DEBUG
			var canApply = this.WhenAny(x => x.SelectedStandart,
				(selectedSt) => selectedSt != null);
#else
			var canApply = this.WhenAnyValue(x => x.SelectedStandart, x => x.IsConfirmed,
				(selectedSt, isConf) => selectedSt != null && isConf);
#endif
			DropWeight_5kg = SendToControllerNumeric(((int)RegAdresses.dropWeight), 5);
			DropWeight_10kg = SendToControllerNumeric(((int)RegAdresses.dropWeight), 10);
			DropWeight_30kg = SendToControllerNumeric(((int)RegAdresses.dropWeight), 30);

            ReactiveCommand<Unit,Unit> SendToControllerNumeric(ushort code, object value){
				return ReactiveCommand.CreateFromTask(async () =>
				{
                    if (device is ModbusTcpDevice modbusTcpDevice && device.GetStatus() == DeviceStatus.Online)
                    {
                        ushort valueConverted = Convert.ToUInt16(value);
                        await modbusTcpDevice.WriteHoldingRegistersAsync(code, new ushort[] { valueConverted });
                    }
				});
			}

			ApplyController = ReactiveCommand.CreateFromTask(async () =>
			{
                await SendToControllerNumeric((ushort)RegAdresses.hydraulicPressureCoef, bench.Settings.TestBenchSettings.HydraulicPressureCoef).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.maxAllowablePressure, bench.Settings.TestBenchSettings.MaxAllowablePressure).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.throttleActivationPercentage, bench.Settings.TestBenchSettings.ThrottleActivationPercentage).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.throttleDeactivationPercentage, bench.Settings.TestBenchSettings.ThrottleDeactivationPercentage).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.overpressureAllowancePercentage, bench.Settings.TestBenchSettings.OverpressureAllowancePercentage).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.nominalPressureValue, bench.Settings.TestBenchSettings.NominalPressureValue).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.reserve1, bench.Settings.TestBenchSettings.Reserve1).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.reserve2, bench.Settings.TestBenchSettings.Reserve2).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.vesselEmergencyLevel_5kg, bench.Settings.TestBenchSettings.VesselEmergencyLevel_5kg).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.vesselEmergencyLevel_10kg, bench.Settings.TestBenchSettings.VesselEmergencyLevel_10kg).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.vesselEmergencyLevel_30kg, bench.Settings.TestBenchSettings.VesselEmergencyLevel_30kg).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.kpPID, bench.Settings.TestBenchSettings.KpPID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.kiPID, bench.Settings.TestBenchSettings.KiPID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.kdPID, bench.Settings.TestBenchSettings.KdPID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.dwePID, bench.Settings.TestBenchSettings.dwePID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.tfPID, bench.Settings.TestBenchSettings.tfPID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.maxOutputConst, bench.Settings.TestBenchSettings.MaxOutputConst).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.errorZonePID, bench.Settings.TestBenchSettings.ErrorZonePID).Execute();
                await SendToControllerNumeric((ushort)RegAdresses.minOutputConst, bench.Settings.TestBenchSettings.MinOutputConst).Execute();
				await SendToControllerNumeric((ushort)RegAdresses.cyclePID, bench.Settings.TestBenchSettings.CyclePID).Execute();
            });

				Apply = ReactiveCommand.CreateFromTask(async () =>
			{
				bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
				bench.Settings.TenzoUnit = (ForceUnit)SelectedTenzoUnit?.UnitType;
				bench.Settings.SelectedBalloon = SelectedBalloon;
				bench.Settings.SelectedBalloon.StandartId = SelectedStandart.Id;
				bench.Settings.SelectedBalloon.BalloonVolume = BalloonVolume;
				bench.Settings.BalloonProperties = BalloonProperties.ToList();
				
				//if (!IsModeAuto)
				//{
				//	testSettings.TenzoSensorId = TenzoSensor.Id;
				//}
				
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

				//Запись сервисных параметров
                bench.Settings.TestBenchSettings.PressureVelocity = serviceParameters.PressureVelocity;
                bench.Settings.TestBenchSettings.HydraulicPressureCoef = serviceParameters.HydraulicPressureCoef;
                bench.Settings.TestBenchSettings.MaxAllowablePressure = serviceParameters.MaxAllowablePressure;
                bench.Settings.TestBenchSettings.ThrottleActivationPercentage = serviceParameters.ThrottleActivationPercentage;
                bench.Settings.TestBenchSettings.ThrottleDeactivationPercentage = serviceParameters.ThrottleDeactivationPercentage;
                bench.Settings.TestBenchSettings.OverpressureAllowancePercentage = serviceParameters.OverpressureAllowancePercentage;
                bench.Settings.TestBenchSettings.NominalPressureValue = serviceParameters.NominalPressureValue;
                bench.Settings.TestBenchSettings.Reserve1 = serviceParameters.Reserve1;
                bench.Settings.TestBenchSettings.Reserve2 = serviceParameters.Reserve2;
                bench.Settings.TestBenchSettings.VesselEmergencyLevel_5kg = serviceParameters.VesselEmergencyLevel_5kg;
                bench.Settings.TestBenchSettings.VesselEmergencyLevel_10kg = serviceParameters.VesselEmergencyLevel_10kg;
                bench.Settings.TestBenchSettings.VesselEmergencyLevel_30kg = serviceParameters.VesselEmergencyLevel_30kg;
                bench.Settings.TestBenchSettings.KpPID = serviceParameters.KpPID;
                bench.Settings.TestBenchSettings.KiPID = serviceParameters.KiPID;
                bench.Settings.TestBenchSettings.KdPID = serviceParameters.KdPID;
                bench.Settings.TestBenchSettings.dwePID = serviceParameters.dwePID;
                bench.Settings.TestBenchSettings.tfPID = serviceParameters.tfPID;
                bench.Settings.TestBenchSettings.MaxOutputConst = serviceParameters.MaxOutputConst;
                bench.Settings.TestBenchSettings.ErrorZonePID = serviceParameters.ErrorZonePID;
                bench.Settings.TestBenchSettings.MinOutputConst = serviceParameters.MinOutputConst;
                bench.Settings.TestBenchSettings.CyclePID = serviceParameters.CyclePID;

                bench.SaveSettings();

				bus.Publish(new App.Benches.SODBench.Messages.SelectedTestMessage());

				navigationService.GoBack();
			}, canApply);

		}

		public ReactiveCommand<Unit, Unit> DropWeight { get; set; }
		[Reactive]
        public ReactiveCommand<Unit, Unit> DropWeight_5kg { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> DropWeight_10kg { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> DropWeight_30kg { get; set; }
        public IEnumerable<IValueViewModel> Properties => parameters.Select(kv => kv.Value);
		[Reactive]
        public TestBenchSettings ServiceProperties
        {
            get => serviceParameters;
            set => serviceParameters = value;
        }
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
        public ReactiveCommand<Unit, Unit> ApplyController { get; set; }
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
		public ISensor TenzoSensor { get; set; }
	}
}
