using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.LeakageSensor.CodeBased
{
    [SensorSettings]
    public class Settings 
    {
        public Settings()
        {
            MinValue = UnitsNet.VolumeFlow.FromCubicCentimetersPerMinute(0);
            MaxValue = UnitsNet.VolumeFlow.FromCubicCentimetersPerMinute(10);
            Unit = VolumeFlowUnit.CubicCentimeterPerMinute;
            Name = MaxValue.ToString();
        }
        public string Name { get; set; }
        public int ChannelId { get; set; }
        public double MaxCode { get; set; }
        public double MinCode { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.VolumeFlow MinValue { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.VolumeFlow MaxValue { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public VolumeFlowUnit Unit { get; set; } = VolumeFlowUnit.CubicCentimeterPerMinute;

        public string Accaury { get; set; } = "F2";
        public double FilterCoef { get; set; } = 0;
        public string SensorHint { get; set; } = "";
    }
}
