using SOD.Core.Device.Modbus;
using SOD.ViewModels.Settings.DeviceAndSensor.Device;
using System.Collections.ObjectModel;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public class ModbusReadWriteBoolRegisterViewModel : ModbusReadWriteRegisterViewModel
    {
        public ModbusReadWriteBoolRegisterViewModel(ObservableCollection<IModbusRegisterViewModel> registers,
                                                    ModbusRegister modbusRegister,
                                                    object modbusTcpDevice,
                                                    bool isHoldingRegister) : base(registers, modbusRegister, modbusTcpDevice, isHoldingRegister)
        {
        }
    }
}
