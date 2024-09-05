using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOD.Core.CustomUnits;
using SOD.Core.Props;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;

namespace SOD.Core.JsonConverters
{
    public class PropertyJsonConverter : JsonConverter<IProperty>
    {
        private UnitsNetIQuantityJsonConverter jsonConverter = new UnitsNetIQuantityJsonConverter();

        public override IProperty ReadJson(JsonReader reader, Type objectType, [AllowNull] IProperty existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var token = JToken.Load(reader);

            //if (!token.HasValues)
            //{
            //return null;
            //}
            
            var jsonObject = JObject.Parse(token.Value<string>());
            var property = jsonObject.ToObject<Property>();

            var serializedValue = jsonObject.GetValue(nameof(property.SerializedValue), StringComparison.OrdinalIgnoreCase);
            var type = jsonObject.GetValue(nameof(property.Type), StringComparison.OrdinalIgnoreCase);

            if (serializedValue == null || type == null)
            {
                return null;
            }

            property.Value = Deserialize(serializedValue.Value<string>(), (PropertyType)type.Value<int>());

            return property;
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] IProperty value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            if (value.Value!=null)
            {
                value.SerializedValue = JsonConvert.SerializeObject(value.Value, Formatting.Indented, jsonConverter);
            }

            var val = JsonConvert.SerializeObject(value, Formatting.Indented, jsonConverter);


            serializer.Serialize(writer, val);
        }


        public object Deserialize(string serializeString, PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.Pressure:
                    return JsonConvert.DeserializeObject<Pressure>(serializeString, jsonConverter);
                case PropertyType.VolumeFlow:
                    return JsonConvert.DeserializeObject<VolumeFlow>(serializeString, jsonConverter);
                case PropertyType.Volume:
                    return JsonConvert.DeserializeObject<Volume>(serializeString, jsonConverter);
                case PropertyType.Area:
                    return JsonConvert.DeserializeObject<Area>(serializeString, jsonConverter);
                case PropertyType.Lenght:
                    return JsonConvert.DeserializeObject<Length>(serializeString, jsonConverter);
                case PropertyType.PNTable:
                    return JsonConvert.DeserializeObject<DinPressureClass>(serializeString, jsonConverter);
                case PropertyType.AnsiClassTable:
                    return JsonConvert.DeserializeObject<AnsiPressureClass>(serializeString, jsonConverter);
                case PropertyType.String:
                    return JsonConvert.DeserializeObject<string>(serializeString, jsonConverter);
                case PropertyType.Double:
                    return JsonConvert.DeserializeObject<double>(serializeString, jsonConverter);
                case PropertyType.Integer:
                    return JsonConvert.DeserializeObject<int>(serializeString, jsonConverter);
                case PropertyType.NPSMetric:
                    return JsonConvert.DeserializeObject<NominalPipeSizeMetric>(serializeString, jsonConverter);
                case PropertyType.NPSInch:
                    return JsonConvert.DeserializeObject<NominalPipeSizeInch>(serializeString, jsonConverter);
                case PropertyType.StringList:
                    return JsonConvert.DeserializeObject<List<object>>(serializeString, jsonConverter);
                default:return null;
            }
        }
    }
}
