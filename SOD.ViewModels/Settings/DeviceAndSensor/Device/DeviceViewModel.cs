using SOD.Core.Device;
using SOD.Core.Device.LCard;
using SOD.Core.Device.Modbus;
using SOD.Localization.Settings.DeviceAndSensors;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class DeviceViewModel : ReactiveObject, IDeviceSensorViewModel
    {
        public DeviceViewModel(IDevice device, INavigationService navigationService, IDialogService dialogService)
        {
            Device = device;
            Name = device.Name;
            SensorHint = device.SensorHint;
            Edit = ReactiveCommand.Create(() =>
            {
                if (device is ModbusTcpDevice modbusTcpDevice)
                {
                    var vm = new ModbusTcpDeviceSettingsViewModel(navigationService, dialogService, modbusTcpDevice);
                    navigationService.NavigateTo("ModbusTcpDeviceSettings", vm);
                }
                //else if(device is E14140Device e14140Device)
                //{
                //    var vm = new E14140DeviceSettingsViewModel(e14140Device, navigationService, dialogService);
                //    navigationService.NavigateTo("E14140DeviceSettings", vm);
                //}
            });
        }
        [Reactive]
        public string Name { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public IDevice Device { get; set; }
        [Reactive]
        public string SensorHint { get; set; }
    }
}
