using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TemperatureSensor.CodeBased
{
    [SensorSettings]
    public class Settings
    {
        public Settings()
        {
            MinValue = UnitsNet.Temperature.FromDegreesCelsius(0);
            MaxValue = UnitsNet.Temperature.FromDegreesCelsius(100);
            Unit = TemperatureUnit.DegreeCelsius;
            Name = MaxValue.ToString();
        }
        public string Name { get; set; }
        public int ChannelId { get; set; }
        public int MaxCode { get; set; }
        public int MinCode { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Temperature MinValue { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Temperature MaxValue { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TemperatureUnit Unit { get; set; } = TemperatureUnit.DegreeCelsius;
        public string Accaury { get; set; } = "F2";
        public double FilterCoef { get; set; } = 0;
        public string SensorHint { get; set; } = "";
    }
}
