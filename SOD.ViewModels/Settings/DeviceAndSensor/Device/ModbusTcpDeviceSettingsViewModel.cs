using SOD.Core.Device.Modbus;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusTcpDeviceSettingsViewModel : ReactiveObject, IActivatableViewModel
    {
        public ModbusTcpDeviceSettingsViewModel(INavigationService navigationService, IDialogService dialogService, ModbusTcpDevice modbusTcpDevice)
        {
            Name = modbusTcpDevice.Settings.DeviceName;
            HostOrIp = modbusTcpDevice.Settings.HostOrIp;
            Port = modbusTcpDevice.Settings.Port;
            SlaveId = modbusTcpDevice.Settings.SlaveAddress;
            GoBack = ReactiveCommand.Create(() => 
            {
                foreach (var reg in Registers)
                {
                    reg.Activator.Deactivate();
                }
                foreach (var reg in ReadWriteRegisters)
                {
                    reg.Activator.Deactivate();
                }
                navigationService.GoBack();
            });
            Save = ReactiveCommand.Create(() =>
            {
                modbusTcpDevice.Disconnect();
                modbusTcpDevice.Settings.DeviceName = Name;
                modbusTcpDevice.Settings.HostOrIp = HostOrIp;
                modbusTcpDevice.Settings.Port = Port;
                modbusTcpDevice.Settings.SlaveAddress = SlaveId;
                modbusTcpDevice.Settings.Registers.Clear();
                modbusTcpDevice.Settings.Registers.AddRange(Registers.Select(r => r.Register));
                modbusTcpDevice.Settings.ReadWriteRegisters.Clear();
                modbusTcpDevice.Settings.ReadWriteRegisters.AddRange(ReadWriteRegisters.Select(r => r.Register));
                modbusTcpDevice.SaveSettings();
                modbusTcpDevice.Connenct();
            });

            Add = ReactiveCommand.CreateFromTask(async () =>
            {
                var modbusRegister = new ModbusRegister();
                if ((bool)await dialogService.ShowDialogAsync("EditModbusTcpRegister", new Dialog.EditModbusTcpRegisterDialogViewModel(dialogService, modbusRegister)))
                {
                    if (modbusRegister.DataType == Core.Device.ChannelDataType.BOOL)
                    {
                        var vm = new ModbusReadWriteBoolRegisterViewModel(ReadWriteRegisters, modbusRegister, modbusTcpDevice);
                        vm.Activator.Activate();
                        ReadWriteRegisters.Add(vm);
                    }
                    else
                    {
                        var vm = new ModbusReadWriteRegisterViewModel(ReadWriteRegisters, modbusRegister, modbusTcpDevice);
                        vm.Activator.Activate();
                        ReadWriteRegisters.Add(vm);
                    }
                }

                
            });

            AddRegister = ReactiveCommand.CreateFromTask(async () =>
            {
                var modbusRegister = new ModbusRegister();
                if ((bool)await dialogService.ShowDialogAsync("EditModbusTcpRegister", new Dialog.EditModbusTcpRegisterDialogViewModel(dialogService, modbusRegister)))
                {
                    var vm = new ModbusTcpRegisterViewModel(modbusTcpDevice, modbusRegister);
                    vm.Activator.Activate();
                    Registers.Add(vm);
                }
            });

            Edit = ReactiveCommand.CreateFromTask(async () =>
            {
                if ((bool)await dialogService.ShowDialogAsync("EditModbusTcpRegister", new Dialog.EditModbusTcpRegisterDialogViewModel(dialogService, SelectedRegister.Register)))
                {
                    SelectedRegister.Id = SelectedRegister.Register.Id;
                    SelectedRegister.DataType = SelectedRegister.Register.DataType;
                    SelectedRegister.Description = SelectedRegister.Register.Description;
                }
            }, this.WhenAny(x => x.SelectedRegister, r => r.Value != null));

            Delete = ReactiveCommand.Create(() =>
            {
                SelectedRegister.Activator.Deactivate();
                Registers.Remove(SelectedRegister);
            }, this.WhenAny(x=>x.SelectedRegister, r=>r.Value!=null));

            modbusTcpDevice.Settings.Registers.ForEach(mr =>
            {
                var vm = new ModbusTcpRegisterViewModel(modbusTcpDevice, mr);
                vm.Activator.Activate();
                Registers.Add(vm);
            });

            modbusTcpDevice.Settings.ReadWriteRegisters.ForEach(mr =>
            {
                if (mr.DataType ==Core.Device.ChannelDataType.BOOL)
                {
                    var vm = new ModbusReadWriteBoolRegisterViewModel(ReadWriteRegisters, mr, modbusTcpDevice);
                    vm.Activator.Activate();
                    ReadWriteRegisters.Add(vm);
                }
                else
                {
                    var vm = new ModbusReadWriteRegisterViewModel(ReadWriteRegisters, mr, modbusTcpDevice);
                    vm.Activator.Activate();
                    ReadWriteRegisters.Add(vm);
                }
            });

            this.WhenActivated(disposables =>
            {
                Observable
                    .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Subscribe(_ =>
                    {
                        modbusTcpDevice.ReadHoldingRegisters(ReadWriteRegisters.Select(r => r.Register).ToArray());
                    })
                    .DisposeWith(disposables);
            });
            
        }
        
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string HostOrIp { get; set; }
        [Reactive]
        public int Port { get; set; }
        [Reactive]
        public int SlaveId { get; set; }
        [Reactive]
        public ModbusTcpRegisterViewModel SelectedRegister { get; set; }
        public ObservableCollection<ModbusTcpRegisterViewModel> Registers { get; set; } = new ObservableCollection<ModbusTcpRegisterViewModel>();
        public ObservableCollection<IModbusRegisterViewModel> ReadWriteRegisters { get; set; } = new ObservableCollection<IModbusRegisterViewModel>();


        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> AddRegister { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public ReactiveCommand<Unit, Unit> Add { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
