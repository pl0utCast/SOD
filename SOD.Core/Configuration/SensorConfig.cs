using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Configuration
{
    public class SensorConfig
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType Type { get; set; }
    }
}
