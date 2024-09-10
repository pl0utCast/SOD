using SOD.Core.Device.Modbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusReadWriteBoolRegisterViewModel : ModbusReadWriteRegisterViewModel
    {
        public ModbusReadWriteBoolRegisterViewModel(ObservableCollection<IModbusRegisterViewModel> registers,
                                                    ModbusRegister modbusRegister,
                                                    ModbusTcpDevice modbusTcpDevice) : base(registers, modbusRegister, modbusTcpDevice)
        {
        }
    }
}
