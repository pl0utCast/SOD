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
                case DeviceType.ModbusTCP:
                    return new Modbus.ModbusTcpDevice(deviceConfig.Id, _settingsService);
                //case DeviceType.E14140:
                    //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    //{
                    //    //return new LCard.E14140DeviceLinux(deviceConfig.Id, _settingsService);
                    //    return null;
                    //}
                    //else
                    //{
                        //return new LCard.E14140Device(deviceConfig.Id, _settingsService);
                        //return new LCard.E14140DeviceWindows(deviceConfig.Id, _settingsService); // для теста библиотечки
                    //}
                //case DeviceType.PKTBAImpulseBoard:
                //    return new PKTBAImpulseSensorBoard.ImpulseSensorBoardDevice(deviceConfig.Id, _settingsService);
                default:
                    return null;
            }
        }
    }
}
