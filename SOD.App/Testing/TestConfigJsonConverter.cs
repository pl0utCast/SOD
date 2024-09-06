using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOD.App.Commands;
using SOD.App.Interfaces;
using SOD.App.Testing.Programms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.App.Testing
{
    public class TestConfigJsonConverter : JsonConverter
    {
        public TestConfigJsonConverter()
        {
            //_jsonConverts = jsonConverts;
        }
        public override bool CanConvert(Type objectType)
        {
            if (typeof(IBranch<ITestConfig>).IsAssignableFrom(objectType)) return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType,  object existingValue, JsonSerializer serializer)
        {
            var t = JObject.Load(reader);
            var props = t.Properties().First();
            if (props.Name==nameof(TestProgrammConfig))
            {
                var testProgram = new TestProgrammConfig();
                foreach (var prop in testProgram.GetType().GetProperties())
                {
                    if (prop.Name != nameof(testProgram.Childrens))
                    {
                        if (prop.Name == nameof(testProgram.Parameters))
                        {
                            //o.Add(prop.Name, JsonConvert.SerializeObject(JArray.FromObject(prop.GetValue(testProgrammConfig), serializer)));
                            var value = t.GetValue(prop.Name);
                            prop.SetValue(testProgram, 
                                JsonConvert.DeserializeObject(t.GetValue(prop.Name).ToObject<string>(), prop.PropertyType, serializer.Converters.ToArray()), 
                                null);
                        }
                        else
                        {
                            prop.SetValue(testProgram, JsonConvert.DeserializeObject(t.GetValue(prop.Name).ToObject<string>(), prop.PropertyType), null);
                        }
                        
                    }
                    
                }
                /*testProgram.Id = JsonConvert.DeserializeObject<int>(t.GetValue(nameof(testProgram.Id)).ToObject<string>());
                testProgram.StandartId = JsonConvert.DeserializeObject<int?>(t.GetValue(nameof(testProgram.StandartId)).ToObject<string>());
                testProgram.TestType = JsonConvert.DeserializeObject<TestType?>(t.GetValue(nameof(testProgram.TestType)).ToObject<string>());
                testProgram.Name = JsonConvert.DeserializeObject<string>(t.GetValue(nameof(testProgram.Name)).ToObject<string>());*/
                testProgram.Childrens = JsonConvert.DeserializeObject<List<IBranch<ITestConfig>>>(t.GetValue(nameof(testProgram.Childrens)).ToObject<string>(), serializer.Converters.ToArray());
                return testProgram;
            }
            else
            {
                var type = Type.GetType(props.Name);
                var val = JsonConvert.DeserializeObject(props.ToObject<string>(), type, new KeyValuePairTypeJsonConverter());
                //((JObject)keyValuePair.Value).ToObject(key);
                return val;
            }
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject o = new JObject();
            if (value is CommandConfig commandConfig)
            {
                var val = JsonConvert.SerializeObject(value, new KeyValuePairTypeJsonConverter());
                o.Add(commandConfig.GetType().FullName, val);
                o.WriteTo(writer);
            }
            if (value is TestProgrammConfig testProgrammConfig)
            {
                o.Add(nameof(TestProgrammConfig), JToken.FromObject(nameof(TestProgrammConfig)));
                foreach (var prop in testProgrammConfig.GetType().GetProperties())
                {
                    if (prop.Name!= nameof(testProgrammConfig.Childrens))
                    {
                        if (prop.Name == nameof(testProgrammConfig.Parameters))
                        {
                            o.Add(prop.Name, JsonConvert.SerializeObject(JArray.FromObject(prop.GetValue(testProgrammConfig),serializer)));
                        }
                        else
                        {
                            o.Add(prop.Name, JsonConvert.SerializeObject(prop.GetValue(testProgrammConfig)));
                        }
                            
                    }
                    
                } 
                /*o.Add(nameof(testProgrammConfig.Id),JsonConvert.SerializeObject(testProgrammConfig.Id));
                o.Add(nameof(testProgrammConfig.StandartId), JsonConvert.SerializeObject(testProgrammConfig.StandartId));
                o.Add(nameof(testProgrammConfig.TestType), JsonConvert.SerializeObject(testProgrammConfig.TestType));
                o.Add(nameof(testProgrammConfig.Name), JsonConvert.SerializeObject(testProgrammConfig.Name));*/
                var childrens = JsonConvert.SerializeObject(testProgrammConfig.Childrens, serializer.Converters.ToArray());
                o.Add(nameof(testProgrammConfig.Childrens), childrens);
                o.WriteTo(writer);
                
            }
        }
    }
}
