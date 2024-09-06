using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;

namespace SOD.App.Testing
{
    class KeyValuePairTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(KeyValuePair<Type, object>)) return true;
            else return false;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = JObject.Load(reader);
            var props = t.Properties().First();
            Type type = null;
            switch (props.Name)
            {
                case nameof(UnitsNet.Pressure):
                    type = typeof(UnitsNet.Pressure);
                    break;
                default:
                    type = Type.GetType(props.Name);
                    break;
            }
            var val = JsonConvert.DeserializeObject(props.ToObject<string>(), type, new JsonSerializerSettings() { Converters = { new UnitsNetJsonConverter() } });
            return new KeyValuePair<Type, object>(type, val);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject o = new JObject();
            var keyValuePair = (KeyValuePair<Type, object>)value;
            if (typeof(IQuantity).IsAssignableFrom(keyValuePair.Key))
            {
                var key = keyValuePair.Key.Name;
                var val = JsonConvert.SerializeObject(keyValuePair.Value, new JsonSerializerSettings() { Converters = { new UnitsNetJsonConverter() } });
                o.Add(key, val);
            }
            else
            {
                var key = keyValuePair.Key.FullName;
                var val = JsonConvert.SerializeObject(keyValuePair.Value);
                o.Add(key, val);
            }
            
            o.WriteTo(writer);
        }
    }
}
