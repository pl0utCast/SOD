using SOD.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface IConfigService
    {
        IEnumerable<SensorConfig> GetSensorConfigs();
        IEnumerable<DeviceConfig> GetDeviceConfigs();
    }
}
