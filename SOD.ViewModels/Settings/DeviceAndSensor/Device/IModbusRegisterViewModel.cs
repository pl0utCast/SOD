using SOD.Core.Device.Modbus;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device
{
    public interface IModbusRegisterViewModel : IActivatableViewModel
    {
        public ModbusRegister Register { get; }
    }
}
