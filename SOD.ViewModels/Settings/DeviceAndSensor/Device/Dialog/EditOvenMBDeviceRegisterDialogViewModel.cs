using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Dialogs;
using SOD.ViewModels.Controls;
using System.Reactive;
using static SOD.Core.Device.OvenMBDevice.Settings;
using static SOD.ViewModels.Settings.DeviceAndSensor.Device.OvenMBDeviceSettingsViewModel;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device.Dialog
{
    public class EditOvenMBDeviceRegisterDialogViewModel : YesNoDialogViewModel
    {
        public EditOvenMBDeviceRegisterDialogViewModel(IDialogService dialogService, 
                                                      Channel channel, 
                                                      List<RegisterViewModel> registers) : base(dialogService)
        {
            Address = channel.Address;
            IsEnabled = channel.IsEnable;

            Save = ReactiveCommand.Create(() =>
            {
                if (Address < 0 || Address > 255)
                {
                    IsAdressError = true;
                    IsUniqueError = false;
                    return;
                }
                else
                {
                    IsAdressError = false;
                    foreach (var channel in registers)
                    {
                        if (channel.Id == Address)
                        {                            
                            IsUniqueError = true;
                            return;
                        }
                    }
                    channel.Address = (byte)Address;
                    channel.IsEnable = IsEnabled;
                    dialogService.CloseAsync(true);
                }
            });

            Cancel = ReactiveCommand.Create(() =>
            {
                dialogService.CloseAsync(false);
            });
        }
        [Reactive]
        public bool IsAdressError { get; set; }
        [Reactive]
        public bool IsUniqueError { get; set; }
        [Reactive]
        public int Address { get; set; }
        [Reactive]
        public bool IsEnabled { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
