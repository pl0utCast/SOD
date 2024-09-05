using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Serialization.JsonNet;

namespace SOD.Core.Props
{
    public class Property : IProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        [JsonIgnore]
        public object Value { get; set; }          
        public string SerializedValue { get; set; }
        public PropertyType Type { get; set; }
    }
}
