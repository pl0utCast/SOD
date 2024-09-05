using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Configuration
{
    public class DeviceConfig
    {
        public int Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType Type { get; set; }
    }
}
