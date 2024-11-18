using LiteDB;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SOD.Core.Valves.Properties;
using UnitsNet;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;
using DynamicData;
using SOD.Core.CustomUnits;
using SOD.Core.Props;
using SOD.Core.Seals;

namespace SOD.Core.Valves
{
    public class ValveSevice : IValveService
    {
        private string dbPath = Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder, "valves.db");
        private string valveCollectionName = "Valves";
        private string valveTypeCollectionName = "ValveType";
        private readonly SourceList<ValveType> valveTypesReactiveList = new SourceList<ValveType>();
        private readonly SourceList<Valve> valveReactiveList = new SourceList<Valve>();
        public ValveSevice()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder));
            }
            var jsonSerializeSettings = new JsonSerializerSettings();
            jsonSerializeSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            BsonMapper.Global.RegisterType<ValveProperty>
                (
                    serialize: (valveProperty) =>
                    {
                        valveProperty.SerializedValue = JsonConvert.SerializeObject(valveProperty.Value, jsonSerializeSettings);
                        return JsonConvert.SerializeObject(valveProperty, jsonSerializeSettings);

                    },
                    deserialize: (bson) =>
                    {
                        var valvePoperty = JsonConvert.DeserializeObject<ValveProperty>(bson.AsString, jsonSerializeSettings);
                        switch (valvePoperty.Type)
                        {
                            case PropertyType.Pressure:
                                valvePoperty.Value = JsonConvert.DeserializeObject<Pressure>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.VolumeFlow:
                                valvePoperty.Value = JsonConvert.DeserializeObject<VolumeFlow>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Volume:
                                valvePoperty.Value = JsonConvert.DeserializeObject<Volume>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Area:
                                valvePoperty.Value = JsonConvert.DeserializeObject<Area>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Lenght:
                                valvePoperty.Value = JsonConvert.DeserializeObject<Length>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.PNTable:
                                valvePoperty.Value = JsonConvert.DeserializeObject<DinPressureClass>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.AnsiClassTable:
                                valvePoperty.Value = JsonConvert.DeserializeObject<AnsiPressureClass>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.String:
                                valvePoperty.Value = JsonConvert.DeserializeObject<string>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Double:
                                valvePoperty.Value = JsonConvert.DeserializeObject<double>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Integer:
                                valvePoperty.Value = JsonConvert.DeserializeObject<int>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSMetric:
                                valvePoperty.Value = JsonConvert.DeserializeObject<NominalPipeSizeMetric>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSInch:
                                valvePoperty.Value = JsonConvert.DeserializeObject<NominalPipeSizeInch>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.StringList:
                                valvePoperty.Value = JsonConvert.DeserializeObject<List<object>>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.SealType:
                                valvePoperty.Value = JsonConvert.DeserializeObject<SealType>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.TestMedium:
                                valvePoperty.Value = JsonConvert.DeserializeObject<TestMediumType>(valvePoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            default:
                                break;
                        }
                        return valvePoperty;
                    }
                );

            BsonMapper.Global.Entity<Valve>()
                .DbRef(v => v.ValveType, valveTypeCollectionName);

            valveTypesReactiveList.AddRange(GetValveTypes());
            valveReactiveList.AddRange(GetValves());
        }

        public void AddOrUpdateValve(Valve valve)
        {
            if (valve == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var storedValve = db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .FindById(valve.Id);
                if (storedValve==null)
                {
                    db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .Insert(valve);
                    valveReactiveList.Add(valve);
                }
                else
                {
                    var rV = valveReactiveList.Items.SingleOrDefault(v => v.Id == valve.Id);
                    if (rV != null)
                    {
                        var index = valveReactiveList.Items.IndexOf(rV);
                        valveReactiveList.Remove(rV);
                        valveReactiveList.Insert(index, valve);
                    }

                    db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .Update(valve);
                    
                }
            }
        }

        public void AddOrUpdateValveType(ValveType valveType)
        {
            if (valveType == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var vType = db.GetCollection<ValveType>(valveTypeCollectionName).FindOne(vt => vt.Id == valveType.Id);
                if (vType!=null)
                {
                    var valves = db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .FindAll()
                        .Where(v => v.ValveType.Id == vType.Id);
                        //.Find(v=>v.ValveType.Id==vType.Id);
                    //db.GetCollection<Valve>(valveCollectionName).FindAll();
                    // ищем какие свойства добавились
                    var newProps = valveType.Properties.Except(vType.Properties).ToList();
                    // ищем какие свойства необходимо удалить
                    var deleteProps = vType.Properties.Except(valveType.Properties).ToList();

                    // убираем и добавляем свойства у типа арматуры
                    foreach (var dp in deleteProps)
                    {
                        vType.DeleteProperty(dp.Id);
                    }
                    foreach (var np in newProps)
                    {
                        vType.AddProperty(np);
                    }

                    foreach (var prop in valveType.Properties)
                    {
                        var editProp = vType.Properties.SingleOrDefault(p => p.Id == prop.Id);
                        editProp.Name = prop.Name;
                        editProp.Prefix = prop.Prefix;
                    }

                    db.GetCollection<ValveType>(valveTypeCollectionName).Update(valveType);
                    var reactiveVt = valveTypesReactiveList.Items.SingleOrDefault(vt => vt.Id == valveType.Id);
                    if (reactiveVt!=null)
                    {
                        valveTypesReactiveList.Replace(reactiveVt, valveType);
                    }

                    // убираем и добавляем свойства у всей арматуры
                    foreach (var valve in valves)
                    {
                        foreach (var dp in deleteProps)
                        {
                            valve.RemoveProperty(dp.Id);
                        }

                        foreach (var np in newProps)
                        {
                            valve.Properties.Add(np);
                        }

                        foreach (var prop in valveType.Properties)
                        {
                            var editProp = valve.Properties.SingleOrDefault(p => p.Id == prop.Id);
                            editProp.Name = prop.Name;
                            editProp.Prefix = prop.Prefix;
                        }
                        db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .Update(valve);
                        var rV = valveReactiveList.Items.SingleOrDefault(v => v.Id == valve.Id);
                        if (rV != null)
                        {
                            valveReactiveList.Remove(rV);
                            valveReactiveList.Add(valve);
                        }
                    }

                }
                else
                {
                    db.GetCollection<ValveType>(valveTypeCollectionName).Insert(valveType);
                    valveTypesReactiveList.Add(valveType);
                }
            }
        }

        public IObservable<IChangeSet<ValveType>> ConnectToValveTypes()
        {
            return valveTypesReactiveList.Connect();
        }

        public IEnumerable<Valve> GetValves()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
               return db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .FindAll()
                        .ToList();
            }
        }

        public IEnumerable<ValveType> GetValveTypes()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                return db.GetCollection<ValveType>(valveTypeCollectionName).FindAll().ToList();
            }
        }

        public void RemoveValve(Valve valve)
        {
            if (valve == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                db.GetCollection<Valve>(valveCollectionName)
                    .Include(v => v.ValveType)
                    .Delete(valve.Id);
                valveReactiveList.Remove(valve);
            }
              
        }

        public void RemoveValveType(ValveType valveType)
        {
            if (valveType == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var valves = db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType)
                        .FindAll()
                        .Where(v => v.ValveType.Id == valveType.Id);
                foreach (var valve in valves)
                {
                    db.GetCollection<Valve>(valveCollectionName)
                        .Include(v => v.ValveType).Delete(valve.Id);
                    valveReactiveList.Remove(valve);
                }
                db.GetCollection<ValveType>(valveTypeCollectionName).Delete(valveType.Id);
                var reactiveVt = valveTypesReactiveList.Items.SingleOrDefault(vt => vt.Id == valveType.Id);
                if (reactiveVt != null)
                {
                    valveTypesReactiveList.Remove(reactiveVt);
                }
            }
        }

        public IObservable<IChangeSet<Valve>> ConnectToValves()
        {
            return valveReactiveList.Connect();
        }
    }
}
