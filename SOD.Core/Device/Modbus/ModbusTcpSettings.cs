using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device.Modbus
{
    [DeviceSettings]
    public class ModbusTcpSettings
    {
        public string DeviceName { get; set; }
        public int Id { get; set; }
        public string HostOrIp { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 502;
        public int SlaveAddress { get; set; } = 1;
        public List<ModbusRegister> Registers { get; set; } = new List<ModbusRegister>();
        public List<ModbusRegister> ReadWriteRegisters { get; set; } = new List<ModbusRegister>();
        public string SensorHint { get; set; } = "";
    }
}
