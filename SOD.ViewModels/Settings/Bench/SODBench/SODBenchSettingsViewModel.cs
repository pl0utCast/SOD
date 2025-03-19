using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.UserService;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Disposables;
using UnitsNet.Units;
using UnitsNet;
using SOD.Core.Units;
using SOD.Core.Sensor.Frenq;
using SOD.Core.Sensor.LeakageSensor.Impulse;
using SOD.Core.Sensor.PressureSensor;
using SOD.Core.Sensor.TemperatureSensor;

namespace SOD.ViewModels.Settings.Bench.SODBench
{
	public class SODBenchSettingsViewModel : ReactiveObject, IActivatableViewModel
	{
		private readonly ISensorService _sensorService;
		private readonly ISettingsService _settingsService;
		private readonly IDialogService _dialogService;
		private readonly ILocalizationService _localizationService;
        private readonly IDeviceService _deviceService;
        private readonly App.Benches.SODBench.Bench _bench;
		private readonly string findReportLocation = Path.Combine(Directory.GetCurrentDirectory()) + @"\reports_template\report_template_sod.frx";
		int i = 0;
		public SODBenchSettingsViewModel(IDialogService dialogService,
										 ISensorService sensorService,
										 App.Benches.SODBench.Bench bench,
										 IUserService userService,
										 ISettingsService settingsService,
										 ILocalizationService localizationService,
                                         IDeviceService deviceService)
		{
			_sensorService = sensorService;
			_settingsService = settingsService;
			_dialogService = dialogService;
			_localizationService = localizationService;
            _deviceService = deviceService;
            _bench = bench;

			if (userService.GetCurrentUser().Role == UserRole.Technologist)
				ItemFromTehnologist = true;
			if (userService.GetCurrentUser().Role == UserRole.Administrator)
			{
				ItemFromUser = true;
				ItemFromTehnologist = true;
			}
			Parameters = new ParametersSettingsViewModel(dialogService, bench.Settings, userService);
			ReportPath = _bench.Settings.ReportPath;
			ReportPathChanged = findReportLocation;
			AutoRange = _bench.Settings.AutoRange;

            foreach (var device in _deviceService.GetAllDevice())
            {
                if (_bench.Settings.DevicesUnits.ContainsKey(device.Id))
                    DevicesUnits.Add(new DeviceViewModel() { Id = device.Id, Name = device.Name, IsEnable = _bench.Settings.DevicesUnits[device.Id] });
            }

            foreach (var sensor in sensorService.GetAllSensors())
			{
				if (_bench.Settings.Sensors.ContainsKey(sensor.Id))
				{
					Sensors.Add(new SensorViewModel() { Id = sensor.Id, Name = sensor.Name, IsEnable = _bench.Settings.Sensors[sensor.Id] });

					if (_bench.Settings.Sensors[sensor.Id] == true)
						ChangeSensorsName.Add(new SensorViewModel() { Id = sensor.Id, Name = sensor.Name, IsEnable = _bench.Settings.Sensors[sensor.Id] });
				}
				else
				{
					Sensors.Add(new SensorViewModel() { Id = sensor.Id, Name = sensor.Name, IsEnable = false });
				}
			}
			this.WhenActivated(dis =>
			{
				Parameters.Activator.Activate().DisposeWith(dis);
			});

			TemperatureSensors = sensorService.GetAllSensors()
											  .Where(s => s is ITemperatureSensor)
											  .Cast<ITemperatureSensor>();
			IsEnableGasTemperatureSensor = bench.Settings.IsEnableGasTemperatureSensor;

			ManualLocation = bench.Settings.ManualLocation;
			IsEnableLiquidTemperatureSensor = bench.Settings.IsEnableLiquidTemperatureSensor;
			SelectedGasTemperatureSensor = TemperatureSensors.FirstOrDefault(s => s.Id == bench.Settings.GasTemperatureSensorId);
			SelectedLiquidTemperatureSensor = TemperatureSensors.FirstOrDefault(s => s.Id == bench.Settings.LiquidTemperatureSensorId);

			PressureUnits = new Pressure().GetUnitTypeInfo();
			SelectedPressureUnit = PressureUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.PressureUnit));

		}
		public void Save()
		{
			_bench.Settings.GasTemperatureSensorId = SelectedGasTemperatureSensor?.Id;
			_bench.Settings.LiquidTemperatureSensorId = SelectedLiquidTemperatureSensor?.Id;
			_bench.Settings.IsEnableGasTemperatureSensor = IsEnableGasTemperatureSensor;
			_bench.Settings.ManualLocation = ManualLocation;
			_bench.Settings.IsEnableLiquidTemperatureSensor = IsEnableLiquidTemperatureSensor;
			if (ManualLocation) _bench.Settings.ReportPath = ReportPath;
			else _bench.Settings.ReportPath = ReportPathChanged;
			_bench.Settings.AutoRange = AutoRange;
			_bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
			_bench.Settings.Sensors.Clear();
			foreach (var sensor in Sensors)
			{
				_bench.Settings.Sensors.Add(sensor.Id, sensor.IsEnable);
			}

            _bench.Settings.DevicesUnits.Clear();
            foreach (var sensor in DevicesUnits)
            {
                _bench.Settings.DevicesUnits.Add(sensor.Id, sensor.IsEnable);
            }

            //Сохраняем имена датчиков 
            i = 0;
			foreach (var sensor in _sensorService.GetAllSensors())
			{
				if (ChangeSensorsName.Count() <= i) break;
				if (ChangeSensorsName[i].Id == sensor.Id)
				{
					if (sensor is PressureSensor pressureSensor)
					{
						pressureSensor.Settings.Name = ChangeSensorsName[i].Name;
						pressureSensor.SaveSettings();
					}
					else if (sensor is TemperatureSensor temperatureSensor)
					{
						temperatureSensor.Settings.Name = ChangeSensorsName[i].Name;
						temperatureSensor.SaveSettings();
					}
					//else if (sensor is FrenqSensor frenqSensor)
					//{
					//	frenqSensor.Settings.Name = ChangeSensorsName[i].Name;
					//	frenqSensor.SaveSettings();
					//}
					//else if (sensor is ImpulseSensor impulseSensor)
					//{
					//	impulseSensor.Settings.Name = ChangeSensorsName[i].Name;
					//	impulseSensor.SaveSettings();
					//}
					else if (sensor is Core.Sensor.PressureSensor.CodeBased.PressureSensor codeBasedPressureSensor)
					{
						codeBasedPressureSensor.Settings.Name = ChangeSensorsName[i].Name;
						codeBasedPressureSensor.SaveSettings();
					}
					else if (sensor is Core.Sensor.LeakageSensor.CodeBased.LeakageSensor codeBasedLeakageSensor)
					{
						codeBasedLeakageSensor.Settings.Name = ChangeSensorsName[i].Name;
						codeBasedLeakageSensor.SaveSettings();
					}
					else if (sensor is Core.Sensor.TemperatureSensor.CodeBased.TemperatureSensor codeBasedTemperatureSensor)
					{
						codeBasedTemperatureSensor.Settings.Name = ChangeSensorsName[i].Name;
						codeBasedTemperatureSensor.SaveSettings();
					}
					//else if (sensor is Core.Sensor.LeakageSensor.MML.LeakageSensor mmleakageSensor)
					//{
					//	mmleakageSensor.Settings.Name = ChangeSensorsName[i].Name;
					//	mmleakageSensor.SaveSettings();
					//}
					i++;
				}
			}
			Parameters.Save();
			_bench?.SaveSettings();
			_dialogService.ShowMessage(_localizationService["Settings.Bench.SodBenchSettingsView.SaveSettings"]);
		}

		public ViewModelActivator Activator { get; } = new ViewModelActivator();

		[Reactive]
		public string ReportPath { get; set; }
		[Reactive]
		public string ReportPathChanged { get; set; }
		public bool ItemFromUser { get; set; } = false;
		public bool ItemFromTehnologist { get; set; } = false; 
		public ObservableCollection<DeviceViewModel> DevicesUnits { get; private set; } = new ObservableCollection<DeviceViewModel>();
        public ParametersSettingsViewModel Parameters { get; set; }
		[Reactive]
		public bool AutoRange { get; set; }
		[Reactive]
		public bool IsEnableLiquidTemperatureSensor { get; set; }
		[Reactive]
		public ITemperatureSensor SelectedLiquidTemperatureSensor { get; set; }
		[Reactive]
		public ITemperatureSensor SelectedGasTemperatureSensor { get; set; }
		[Reactive]
		public bool IsEnableGasTemperatureSensor { get; set; }
		public IEnumerable<ITemperatureSensor> TemperatureSensors { get; set; }
		public ObservableCollection<SensorViewModel> Sensors { get; set; } = new ObservableCollection<SensorViewModel>();
		public ObservableCollection<SensorViewModel> ChangeSensorsName { get; set; } = new ObservableCollection<SensorViewModel>();
		[Reactive]
		public bool ManualLocation { get; set; }
		[Reactive]
		public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
		[Reactive]
		public UnitTypeInfo SelectedPressureUnit { get; set; }
		public class SensorViewModel
		{
			public int Id { get; set; }
			public string Name { get; set; }
			[Reactive]
			public bool IsEnable { get; set; }
		}

        public class DeviceViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [Reactive]
            public bool IsEnable { get; set; }
        }
    }
}
