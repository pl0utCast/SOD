using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.Core.Settings
{
    public class TypeObjectDictonaryJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Dictionary<Type, object>)) return true;
            else return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = JObject.Load(reader);
            var dic = t.ToObject<Dictionary<Type, object>>();
            for (int i = 0; i < dic.Keys.Count; i++)
            {
                var key = dic.Keys.ElementAt(i);
                dic[key] = ((JObject)dic[key]).ToObject(key);
            }
            return dic;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = new JObject();
                var dic = (Dictionary<Type, object>)value;
                foreach (var item in dic)
                {
                    var key = JObject.FromObject(item);
                    var serializeValue = JObject.FromObject(item.Value);
                    o.Add(key.Properties().First().Value.ToString(), serializeValue);
                }
                o.WriteTo(writer);
            }
        }
    }
}
