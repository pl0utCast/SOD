using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SOD.Core.JsonConverters
{
    public class DataSetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (typeof(DataSet) == objectType) return true;
            else return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            var dataSet = new DataSet();
            foreach (var prop in o.Properties())
            {
                var tableName = prop.Name;
                var table = new DataTable(tableName);
                foreach (var item in prop.Value.First)
                {
                    var obj = (JProperty)item;
                    if (obj.Value.ToString().Contains("Image:"))
                    {
                        table.Columns.Add(new DataColumn(obj.Name, typeof(Image)));
                    }
                    else
                    {
                        table.Columns.Add(new DataColumn(obj.Name));
                    }
                }

                foreach (var row in prop.Value)
                {
                    var list = new List<object>();
                    foreach (var item in row)
                    {
                        var jsonVal = ((JProperty)item).Value.ToString();
                        if (jsonVal.Contains("Image:"))
                        {
                            jsonVal = jsonVal.Replace("Image:","");
                            list.Add(Image.FromStream(new MemoryStream(Convert.FromBase64String(jsonVal))));
                        }
                        else
                        {
                            list.Add(jsonVal);
                        }
                    }
                    table.Rows.Add(list.ToArray());
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
