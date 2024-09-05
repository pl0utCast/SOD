using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TemperatureSensor
{
    [SensorSettings]
    public class TemperartureSensorSettings
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public TemperatureUnit Unit { get; set; } = TemperatureUnit.DegreeCelsius;
        public string Name { get; set; }
        public string Accaury { get; set; } = "F2";
        public double FilterCoef { get; set; } = 0;
        public string SensorHint { get; set; } = "";
    }
}
