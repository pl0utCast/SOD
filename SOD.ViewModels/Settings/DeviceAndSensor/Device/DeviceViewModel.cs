using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Device;
using SOD.Core.Device.Controllers;
using SOD.Core.Device.Modbus;
using SOD.Core.Device.OvenMBDevice;
using SOD.Dialogs;
using SOD.Navigation;
using System.Reactive;

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
                if (device is OvenMBDevice ovenMBDevice)
                {
                    var vm = new OvenMBDeviceSettingsViewModel(navigationService, dialogService, ovenMBDevice);
                    navigationService.NavigateTo("OvenMBDeviceSettings", vm);
                }
                else if (device is ModbusTcpDevice modbusTcpDevice)
                {
                    var vm = new ModbusTcpDeviceSettingsViewModel(navigationService, dialogService, modbusTcpDevice);
                    navigationService.NavigateTo("ModbusTcpDeviceSettings", vm);
                }
                else if (device is ICPConDevice icpConDevice)
                {
                    var vm = new ICPConDeviceSettingsViewModel(navigationService, dialogService, icpConDevice);
                    navigationService.NavigateTo("ICPConDeviceSettings", vm);
                }
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
