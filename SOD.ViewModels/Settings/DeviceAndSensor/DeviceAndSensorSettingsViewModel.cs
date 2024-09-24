using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.ViewModels.Settings.DeviceAndSensor.Device;
using SOD.ViewModels.Settings.DeviceAndSensor.Sensors;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor
{
    public class DeviceAndSensorSettingsViewModel : ReactiveObject, IActivatableViewModel
    {
        public DeviceAndSensorSettingsViewModel(INavigationService navigationService, IDeviceService deviceService, ISensorService sensorService, IDialogService dialogService, ILocalizationService localizationService)
        {
            ViewTitle = localizationService["Settings.DevicesAndSensors"];
            Devices = deviceService.GetAllDevice().Select(d => new DeviceViewModel(d, navigationService, dialogService));
            foreach (var sensor in sensorService.GetAllSensors())
            {
                if (sensor is IPressureSensor pressureSensor)
                {
                    PressureSensors.Add(new SensorViewModel(sensor, dialogService));
                }
                else if (sensor is ITemperatureSensor temperatureSensor)
                {
                    TemperatureSensors.Add(new SensorViewModel(sensor, dialogService));
                }
                //else if (sensor is ILeakageSensor leakageSensor)
                //{
                //    LeakageSensors.Add(new SensorViewModel(sensor, dialogService));
                //}
            }
            
            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
        }
        public IEnumerable<DeviceViewModel> Devices { get; set; }
        public List<SensorViewModel> PressureSensors { get; set; } = new List<SensorViewModel>();
        public List<SensorViewModel> TemperatureSensors { get; set; } = new List<SensorViewModel>();
        //public List<SensorViewModel> LeakageSensors { get; set; } = new List<SensorViewModel>();
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public string ViewTitle { get; set; }
    }
}
