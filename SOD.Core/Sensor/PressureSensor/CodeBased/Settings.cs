using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.PressureSensor.CodeBased
{
    [SensorSettings]
    public class Settings
    {
        public Settings()
        {
            MinValue = UnitsNet.Pressure.FromBars(0);
            MaxValue = UnitsNet.Pressure.FromBars(100);
            Unit = PressureUnit.Bar;
            Name = MaxValue.ToString(); 
        }
        public string Name { get; set; }
        public int ChannelId { get; set; }
        public int MaxCode { get; set; }
        public int MinCode { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Pressure MinValue { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Pressure MaxValue { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PressureUnit Unit { get; set; } = PressureUnit.Bar;
        public string Accaury { get; set; } = "F2";
        public double FilterCoef { get; set; } = 0;
        public string SensorHint { get; set; } = "";
    }
}
