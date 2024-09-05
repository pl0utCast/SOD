using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.Core.JsonConverters
{
    public class KeyValuePairJsonConverter : JsonConverter
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
            var type = Type.GetType(props.Name);
            var val = JsonConvert.DeserializeObject(props.ToObject<string>(), type);
                //((JObject)keyValuePair.Value).ToObject(key);
            return new KeyValuePair<Type, object>(type, val);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject o = new JObject();
            var keyValuePair = (KeyValuePair<Type, object>)value;
            var key = keyValuePair.Key.FullName;
            var val = JsonConvert.SerializeObject(keyValuePair.Value);
            o.Add(key, val);
            o.WriteTo(writer);
        }
    }
}
