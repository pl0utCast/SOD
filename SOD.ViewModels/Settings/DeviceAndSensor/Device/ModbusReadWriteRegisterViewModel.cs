using SOD.Core.Device;
using SOD.Core.Device.Modbus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusReadWriteRegisterViewModel : ReactiveObject, IModbusRegisterViewModel, IActivatableViewModel
    {
        private bool isWrite = false;
        private ModbusTcpDevice _modbusTcpDevice;
        public ModbusReadWriteRegisterViewModel(ObservableCollection<IModbusRegisterViewModel> registers, ModbusRegister modbusRegister, ModbusTcpDevice modbusTcpDevice)
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
                modbusTcpDevice.DataComplite
                    .Where(r=>r.Id==Id)
                    .Subscribe(r =>
                    {
                        val = r.Value;
                    })
                    .DisposeWith(disposables);
            });
            _modbusTcpDevice.ReadHoldingRegisters(new[] { Register });
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
                if (val!=null) _modbusTcpDevice.WriteHoldingRegister(Register);
                this.RaisePropertyChanged();
            }
        }
        public ChannelDataType DataType { get; set; }
        public ModbusRegister Register { get; private set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
