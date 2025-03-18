using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Device;
using SOD.Core.Device.Controllers;
using SOD.Core.Device.Modbus;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusReadWriteRegisterViewModel : ReactiveObject, IModbusRegisterViewModel, IActivatableViewModel
    {
        private bool isWrite = false;
        private object _modbusTcpDevice;

        public ModbusReadWriteRegisterViewModel(
            ObservableCollection<IModbusRegisterViewModel> registers,
            ModbusRegister modbusRegister,
            object modbusTcpDevice,
            bool isHoldingRegister)
        {
            _modbusTcpDevice = modbusTcpDevice;
            Register = modbusRegister;
            Id = modbusRegister.Id;
            Description = modbusRegister.Description;
            DataType = modbusRegister.DataType;

            Delete = ReactiveCommand.Create(() =>
            {
                registers.Remove(this);
                Activator.Deactivate();
            });

            this.WhenActivated(disposables =>
            {
                if (_modbusTcpDevice is ModbusTcpDevice modbusTcpDevice)
                {
                    modbusTcpDevice.DataComplite
                                   .Where(r => r.Id == Id)
                                   .Subscribe(r =>
                                   {
                                       val = r.Value;
                                   })
                                   .DisposeWith(disposables);
                }
                else if (_modbusTcpDevice is ICPConDevice icpConDevice)
                {
                    icpConDevice.DataComplite
                                .Where(r => r.Id == Id)
                                .Subscribe(r =>
                                {
                                    val = r.Value;
                                })
                                .DisposeWith(disposables);
                }
            });

            if (_modbusTcpDevice is ModbusTcpDevice modbusDevice)
            {
                modbusDevice.ReadHoldingRegisters(new[] { Register });
            }
            else if (_modbusTcpDevice is ICPConDevice conDevice)
            {
                if (Register.DataType == ChannelDataType.BOOL)
                    conDevice.ReadCoils((ushort)Register.Id, 1);
                else
                {
                    if (isHoldingRegister)
                        conDevice.ReadHoldingRegistersRequest(new[] { Register });
                    else
                        conDevice.ReadInputRegisters(new[] { Register });
                }
            }
            val = modbusRegister.Value;
        }

        [Reactive]
        public string Description { get; set; }
        [Reactive]
        public int Id { get; set; }

        private object val;
        public object Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
                Register.Value = val;
                if (val != null)
                {
                    if (_modbusTcpDevice is ModbusTcpDevice modbusDevice)
                    {
                        modbusDevice.WriteHoldingRegister(Register);
                    }
                    else if (_modbusTcpDevice is ICPConDevice conDevice)
                    {
                        if (Register.DataType == ChannelDataType.BOOL)
                            Task.Run(async () => await conDevice.WriteSingleCoilAsync((ushort)Register.Id, (bool)Register.Value));
                        else
                            conDevice.WriteHoldingRegister(Register);
                    }
                }
                this.RaisePropertyChanged();
            }
        }
        public ChannelDataType DataType { get; set; }
        public ModbusRegister Register { get; private set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
