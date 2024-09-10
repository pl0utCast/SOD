using SOD.Core.Device;
using SOD.Core.Device.Modbus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusTcpRegisterViewModel : ReactiveObject, IActivatableViewModel
    {
        public ModbusTcpRegisterViewModel(ModbusTcpDevice modbusTcpDevice, ModbusRegister modbusRegister)
        {
            Register = modbusRegister;
            Id = modbusRegister.Id;
            DataType = modbusRegister.DataType;
            Value = modbusRegister.Value?.ToString();
            Description = modbusRegister.Description;

            this.WhenActivated(disposables =>
            {
                modbusTcpDevice.DataComplite.Subscribe(dc =>
                {
                    if (dc.Id == Id)
                    {
                        Value = dc.Value.ToString();
                    }
                }).DisposeWith(disposables);
            });
        }
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        public ChannelDataType DataType { get; set; }
        [Reactive]
        public string Value { get; set; }
        public ModbusRegister Register { get; set; }
        [Reactive]
        public string Description { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
