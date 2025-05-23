using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Device;
using SOD.Core.Device.Controllers;
using SOD.Core.Device.Modbus;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusTcpRegisterViewModel : ReactiveObject, IActivatableViewModel
    {
        public ModbusTcpRegisterViewModel(object modbusTcpDevice, ModbusRegister modbusRegister)
        {
            Register = modbusRegister;
            Id = modbusRegister.Id;
            DataType = modbusRegister.DataType;
            Value = modbusRegister.Value?.ToString();
            Description = modbusRegister.Description;

            this.WhenActivated(disposables =>
            {
                if (modbusTcpDevice is ModbusTcpDevice modbusDevice)
                {
                    modbusDevice.DataComplite.Subscribe(dc =>
                    {
                        if (dc.Id == Id)
                        {
                            Value = dc.Value.ToString();

                            // Преобразуем значение в двоичное
                            if (dc.DataType == ChannelDataType.UINT16)
                                BinValue = Convert.ToString((ushort)dc.Value, 2).PadLeft(16, '0');
                            else if (dc.DataType == ChannelDataType.INT16)
                                BinValue = Convert.ToString((short)dc.Value, 2).PadLeft(16, '0');
                            else if (dc.DataType == ChannelDataType.UINT)
                                BinValue = Convert.ToString((short)dc.Value, 2).PadLeft(16, '0');
                        }
                    }).DisposeWith(disposables);
                }
                else if (modbusTcpDevice is ICPConDevice conDevice)
                {
                    conDevice.DataComplite.Subscribe(dc =>
                    {
                        if (dc.Id == Id)
                        {
                            Value = dc.Value.ToString();
                        }
                    }).DisposeWith(disposables);
                }
            });
        }
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        public ChannelDataType DataType { get; set; }
        [Reactive]
        public string Value { get; set; }
        [Reactive]
        public string BinValue { get; set; }
        public ModbusRegister Register { get; set; }
        [Reactive]
        public string Description { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
