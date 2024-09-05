using Newtonsoft.Json;
using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.LeakageSensor.MML
{
    [SensorSettings]
    public class Settings
    {
        public string Name { get; set; }
        public List<DependencySensorSettings> SensorsSettings { get; set; } = new List<DependencySensorSettings>();
        public int SwitchDelay { get; set; } = 500;
        public VolumeFlowUnit Unit { get; set; } = VolumeFlowUnit.CubicCentimeterPerMinute;
        public string Accaury { get; set; } = "F2";
        public int LogicSwitchMIP { get; set; } = 0;
        public class DependencySensorSettings
        {
            public int Id { get; set; }
            public int DiscreteChannelId { get; set; }
            [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
            public VolumeFlow MinValue { get; set; }
            [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
            public VolumeFlow MaxValue { get; set; }
            public TimeSpan SwitchDelay { get; set; } = TimeSpan.FromMilliseconds(500);
        }
        public string SensorHint { get; set; } = "";
    }
}
