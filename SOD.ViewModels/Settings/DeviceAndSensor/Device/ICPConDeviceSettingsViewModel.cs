using SOD.Core.Device.Controllers;
using SOD.Core.Device.Modbus;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ICPConDeviceSettingsViewModel : ReactiveObject, IActivatableViewModel
    {
        public ICPConDeviceSettingsViewModel(INavigationService navigationService, IDialogService dialogService, ICPConDevice modbusTcpDevice)
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
                foreach (var reg in ReadHoldingRegisters)
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
                modbusTcpDevice.Settings.ReadHoldingRegisters.Clear();
                modbusTcpDevice.Settings.ReadHoldingRegisters.AddRange(ReadHoldingRegisters.Select(r => r.Register));
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
                        var vm = new ModbusReadWriteBoolRegisterViewModel(ReadWriteRegisters, modbusRegister, modbusTcpDevice, false);
                        vm.Activator.Activate();
                        ReadWriteRegisters.Add(vm);
                    }
                    else
                    {
                        var vm = new ModbusReadWriteRegisterViewModel(ReadWriteRegisters, modbusRegister, modbusTcpDevice, false);
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

            AddHoldingRegister = ReactiveCommand.CreateFromTask(async () =>
            {
                var modbusRegister = new ModbusRegister();
                if ((bool)await dialogService.ShowDialogAsync("EditModbusTcpRegister", new Dialog.EditModbusTcpRegisterDialogViewModel(dialogService, modbusRegister)))
                {
                    if (modbusRegister.DataType == Core.Device.ChannelDataType.BOOL)
                    {
                        var vm = new ModbusReadWriteBoolRegisterViewModel(ReadHoldingRegisters, modbusRegister, modbusTcpDevice, true);
                        vm.Activator.Activate();
                        ReadHoldingRegisters.Add(vm);
                    }
                    else
                    {
                        var vm = new ModbusReadWriteRegisterViewModel(ReadHoldingRegisters, modbusRegister, modbusTcpDevice, true);
                        vm.Activator.Activate();
                        ReadHoldingRegisters.Add(vm);
                    }
                }
            });

            EditHoldingRegister = ReactiveCommand.CreateFromTask(async () =>
            {
                if ((bool)await dialogService.ShowDialogAsync("EditModbusTcpRegister", new Dialog.EditModbusTcpRegisterDialogViewModel(dialogService, SelectedHoldingRegister.Register)))
                {
                    SelectedHoldingRegister.Id = SelectedHoldingRegister.Register.Id;
                    SelectedHoldingRegister.DataType = SelectedHoldingRegister.Register.DataType;
                    SelectedHoldingRegister.Description = SelectedHoldingRegister.Register.Description;
                }
            }, this.WhenAny(x => x.SelectedHoldingRegister, r => r.Value != null));

            DeleteHoldingRegister = ReactiveCommand.Create(() =>
            {
                SelectedHoldingRegister.Activator.Deactivate();
                Registers.Remove(SelectedHoldingRegister);
            }, this.WhenAny(x => x.SelectedHoldingRegister, r => r.Value != null));

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
            }, this.WhenAny(x => x.SelectedRegister, r => r.Value != null));

            modbusTcpDevice.Settings.Registers.ForEach(mr =>
            {
                var vm = new ModbusTcpRegisterViewModel(modbusTcpDevice, mr);
                vm.Activator.Activate();
                Registers.Add(vm);
            });

            modbusTcpDevice.Settings.ReadWriteRegisters.ForEach(mr =>
            {
                if (mr.DataType == Core.Device.ChannelDataType.BOOL)
                {
                    var vm = new ModbusReadWriteBoolRegisterViewModel(ReadWriteRegisters, mr, modbusTcpDevice, false);
                    vm.Activator.Activate();
                    ReadWriteRegisters.Add(vm);
                }
                else
                {
                    var vm = new ModbusReadWriteRegisterViewModel(ReadWriteRegisters, mr, modbusTcpDevice, false);
                    vm.Activator.Activate();
                    ReadWriteRegisters.Add(vm);
                }
            });

            modbusTcpDevice.Settings.ReadHoldingRegisters.ForEach(mr =>
            {
                if (mr.DataType == Core.Device.ChannelDataType.BOOL)
                {
                    var vm = new ModbusReadWriteBoolRegisterViewModel(ReadHoldingRegisters, mr, modbusTcpDevice, true);
                    vm.Activator.Activate();
                    ReadHoldingRegisters.Add(vm);
                }
                else
                {
                    var vm = new ModbusReadWriteRegisterViewModel(ReadHoldingRegisters, mr, modbusTcpDevice, true);
                    vm.Activator.Activate();
                    ReadHoldingRegisters.Add(vm);
                }
            });

            OpenWebDevice = ReactiveCommand.Create(() =>
            {
                if (HostOrIp == null) return;
                var url = $"http://{HostOrIp}";

                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string explorerLocation = "%programfiles%\\Internet Explorer\\iexplore.exe";
                        explorerLocation = Environment.ExpandEnvironmentVariables(explorerLocation);

                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = explorerLocation,
                            Arguments = url
                        });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            this.WhenActivated(disposables =>
            {
                Observable
                    .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Subscribe(_ =>
                    {
                        modbusTcpDevice.ReadInputRegisters(ReadWriteRegisters.Select(r => r.Register).ToArray());
                        modbusTcpDevice.ReadHoldingRegistersRequest(ReadHoldingRegisters.Select(r => r.Register).ToArray());
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
        [Reactive]
        public ModbusTcpRegisterViewModel SelectedHoldingRegister { get; set; }
        public ObservableCollection<ModbusTcpRegisterViewModel> Registers { get; set; } = new ObservableCollection<ModbusTcpRegisterViewModel>();
        public ObservableCollection<IModbusRegisterViewModel> ReadWriteRegisters { get; set; } = new ObservableCollection<IModbusRegisterViewModel>();
        public ObservableCollection<IModbusRegisterViewModel> ReadHoldingRegisters { get; set; } = new ObservableCollection<IModbusRegisterViewModel>();
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> AddRegister { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> AddHoldingRegister { get; set; }
        public ReactiveCommand<Unit, Unit> EditHoldingRegister { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteHoldingRegister { get; set; }
        public ReactiveCommand<Unit, Unit> OpenWebDevice { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
