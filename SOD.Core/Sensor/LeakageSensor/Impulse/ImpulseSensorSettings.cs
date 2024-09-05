using Newtonsoft.Json;
using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.LeakageSensor.Impulse
{
    [SensorSettings]
    public class ImpulseSensorSettings
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public VolumeFlowUnit FlowUnit { get; set; }
        public VolumeUnit VolumeUnit { get; set; } = VolumeUnit.CubicCentimeter;
        public Volume ImpulsePrice { get; set; } = new Volume(1, VolumeUnit.CubicCentimeter);
        public string Name { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public VolumeFlow MaxValue { get; set; }

        public string Accaury { get; set; } = "F2";
        public double FilterCoef { get; set; } = 0;
        public string SensorHint { get; set; } = "";
    }
}
