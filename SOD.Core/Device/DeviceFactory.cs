using SOD.Core.Configuration;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SOD.Core.Device
{
    public class DeviceFactory
    {
        private ISettingsService _settingsService;
        public DeviceFactory(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public IDevice CreateDevice(DeviceConfig deviceConfig)
        {
            if (deviceConfig == null) return null;
            switch (deviceConfig.Type)
            {
                case DeviceType.OvenMBDevice:
                    return new OvenMBDevice.OvenMBDevice(deviceConfig.Id, _settingsService);
                case DeviceType.ModbusTCP:
                    return new Modbus.ModbusTcpDevice(deviceConfig.Id, _settingsService);
                case DeviceType.ICPConDevice:
                    return new Controllers.ICPConDevice(deviceConfig.Id, _settingsService);
                default:
                    return null;
            }
        }
    }
}
