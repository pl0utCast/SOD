using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Sensor
{
    [SensorSettings]
    public class LastUpadateSensorSettings
    {
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
