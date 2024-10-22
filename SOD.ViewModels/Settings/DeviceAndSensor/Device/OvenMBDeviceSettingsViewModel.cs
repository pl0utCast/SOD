using SOD.Core.Device.OvenMBDevice;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using static SOD.Core.Device.OvenMBDevice.Settings;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class OvenMBDeviceSettingsViewModel : ReactiveObject
    {
        public OvenMBDeviceSettingsViewModel(INavigationService navigationService,
                                             IDialogService dialogService,
                                             OvenMBDevice impulseSensorBoardDevice)
        {
            Name = impulseSensorBoardDevice.Settings.Name;
            ComName = impulseSensorBoardDevice.Settings.SerialPort;
            UsedBaudRate = impulseSensorBoardDevice.Settings.UsedBaudRate;
            UseDigitalSensors = impulseSensorBoardDevice.Settings.UseDigitalSensors;
            foreach (var channel in impulseSensorBoardDevice.Settings.RequestChannels)
            {
                var vm = new RegisterViewModel(impulseSensorBoardDevice, dialogService) { Id = channel.Address, IsEnable = channel.IsEnable };
                vm.Activator.Activate();
                Registers.Add(vm);
            }

            Add = ReactiveCommand.CreateFromTask(async () =>
            {
                var reg = Registers.ToList();
                reg.Sort();
                var newChannel = new Channel((byte)(reg.Last().Id + 1), false);
                if ((bool)await dialogService.ShowDialogAsync("EditOvenMBDeviceRegister", 
                                                            new Dialog.EditOvenMBDeviceRegisterDialogViewModel(dialogService, newChannel, reg)))
                {
                    var vm = new RegisterViewModel(impulseSensorBoardDevice, dialogService) { Id = newChannel.Address, IsEnable = newChannel.IsEnable };
                    vm.Activator.Activate();
                    Registers.Add(vm);
                }
            });

            Edit = ReactiveCommand.CreateFromTask(async () =>
            {
                var oldId = (byte)SelectedRegister.Id;
                var channel = new Channel(oldId, SelectedRegister.IsEnable);
                var reg = Registers.ToList();
                reg.Remove(reg.First(x => x.Id == oldId));
                if ((bool)await dialogService.ShowDialogAsync("EditOvenMBDeviceRegister", 
                                                            new Dialog.EditOvenMBDeviceRegisterDialogViewModel(dialogService, channel, reg)))
                {
                    SelectedRegister.Id = channel.Address;
                    SelectedRegister.IsEnable = channel.IsEnable;
                }
            }, this.WhenAny(x => x.SelectedRegister, r => r.Value != null));

            Delete = ReactiveCommand.Create(() =>
            {
                var registr = Registers.First(x => x.Id == SelectedRegister.Id);
                Registers.Remove(registr);
                SelectedRegister = Registers.First();
            }, this.WhenAny(x => x.SelectedRegister, r => r.Value != null));

            Save = ReactiveCommand.Create(() =>
            {
                impulseSensorBoardDevice.Settings.RequestChannels.Clear();
                foreach (var channel in Registers)
                {
                    impulseSensorBoardDevice.Settings.RequestChannels.Add(new Channel((byte)channel.Id, channel.IsEnable));
                    channel.Activator.Deactivate();                    
                }
                impulseSensorBoardDevice.Settings.Name = Name;
                impulseSensorBoardDevice.Settings.SerialPort = ComName;
                impulseSensorBoardDevice.Settings.UsedBaudRate = UsedBaudRate;
                impulseSensorBoardDevice.Settings.UseDigitalSensors = UseDigitalSensors;
                impulseSensorBoardDevice.SaveSettings();
            });

            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
        }
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string ComName { get; set; }
        [Reactive]
        public int UsedBaudRate { get; set; }
        [Reactive]
        public bool UseDigitalSensors { get; set; }
        public ObservableCollection<RegisterViewModel> Registers { get; set; } = new ObservableCollection<RegisterViewModel>();
        [Reactive]
        public RegisterViewModel SelectedRegister { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }


        public class RegisterViewModel : ReactiveObject, IActivatableViewModel, IComparable<RegisterViewModel>
        {
            private object currentValue;
            public RegisterViewModel(OvenMBDevice impulseSensorBoardDevice,
                                     IDialogService dialogService)
            {
                this.WhenActivated(dis =>
                {
                    impulseSensorBoardDevice.DataComplite.Subscribe(c =>
                    {
                        if (c.Id==Id)
                        {
                            Value = c.Value;
                        }
                    }).DisposeWith(dis);

                });
            }
            public int CompareTo(RegisterViewModel obj)
            {
                return this.Id.CompareTo(obj.Id);
            }

            [Reactive]
            public int Id { get; set; }
            [Reactive]
            public bool IsEnable { get; set; }
            [Reactive]
            public object Value { get; set; }
            public ReactiveCommand<Unit, Unit> Reset { get; set; }

            public ViewModelActivator Activator { get; } = new ViewModelActivator();
        }
    }
}
