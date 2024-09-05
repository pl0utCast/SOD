using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Props
{
    public interface IProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public PropertyType Type { get; set; }
        [JsonIgnore]
        public object Value { get; set; }
        public string SerializedValue { get; set; }
    }
}
