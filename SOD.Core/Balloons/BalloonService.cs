using LiteDB;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SOD.Core.Balloons.Properties;
using UnitsNet;
using Newtonsoft.Json;
using UnitsNet.Serialization.JsonNet;
using DynamicData;
using SOD.Core.CustomUnits;
using SOD.Core.Props;
using SOD.Core.Seals;

namespace SOD.Core.Balloons
{
    public class BalloonService : IBalloonService
    {
        private string dbPath = Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder, "balloons.db");
        private string balloonCollectionName = "Balloons";
        private string balloonTypeCollectionName = "BalloonType";
        private readonly SourceList<BalloonType> balloonTypesReactiveList = new SourceList<BalloonType>();
        private readonly SourceList<Balloon> balloonReactiveList = new SourceList<Balloon>();
        public BalloonService()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder));
            }
            var jsonSerializeSettings = new JsonSerializerSettings();
            jsonSerializeSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            BsonMapper.Global.RegisterType<BalloonProperty>
                (
                    serialize: (balloonProperty) =>
                    {
                        balloonProperty.SerializedValue = JsonConvert.SerializeObject(balloonProperty.Value, jsonSerializeSettings);
                        return JsonConvert.SerializeObject(balloonProperty, jsonSerializeSettings);

                    },
                    deserialize: (bson) =>
                    {
                        var balloonPoperty = JsonConvert.DeserializeObject<BalloonProperty>(bson.AsString, jsonSerializeSettings);
                        switch (balloonPoperty.Type)
                        {
                            case PropertyType.Pressure:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<Pressure>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.VolumeFlow:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<VolumeFlow>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Volume:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<Volume>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Area:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<Area>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Lenght:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<Length>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.PNTable:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<DinPressureClass>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.AnsiClassTable:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<AnsiPressureClass>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.String:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<string>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Double:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<double>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Integer:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<int>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSMetric:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<NominalPipeSizeMetric>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSInch:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<NominalPipeSizeInch>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.StringList:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<List<object>>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.SealType:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<SealType>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.TestMedium:
                                balloonPoperty.Value = JsonConvert.DeserializeObject<TestMediumType>(balloonPoperty.SerializedValue, jsonSerializeSettings);
                                break;
                            default:
                                break;
                        }
                        return balloonPoperty;
                    }
                );

            BsonMapper.Global.Entity<Balloon>()
                .DbRef(v => v.BalloonType, balloonTypeCollectionName);

            balloonTypesReactiveList.AddRange(GetBalloonTypes());
            balloonReactiveList.AddRange(GetBalloons());
        }

        public void AddOrUpdateBalloon(Balloon balloon)
        {
            if (balloon == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var storedBalloon = db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .FindById(balloon.Id);

                if (storedBalloon==null)
                {
                    db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .Insert(balloon);
                    balloonReactiveList.Add(balloon);
                }
                else
                {
                    var rV = balloonReactiveList.Items.SingleOrDefault(v => v.Id == balloon.Id);
                    if (rV != null)
                    {
                        var index = balloonReactiveList.Items.IndexOf(rV);
                        balloonReactiveList.Remove(rV);
                        balloonReactiveList.Insert(index, balloon);
                    }

                    db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .Update(balloon);
                    
                }
            }
        }

        public void AddOrUpdateBalloonType(BalloonType balloonType)
        {
            if (balloonType == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var vType = db.GetCollection<BalloonType>(balloonTypeCollectionName).FindOne(vt => vt.Id == balloonType.Id);
                if (vType!=null)
                {
                    var balloons = db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .FindAll()
                        .Where(v => v.BalloonType.Id == vType.Id);
                        //.Find(v=>v.BalloonType.Id==vType.Id);
                    //db.GetCollection<Balloon>(balloonCollectionName).FindAll();
                    // ищем какие свойства добавились
                    var newProps = balloonType.Properties.Except(vType.Properties).ToList();
                    // ищем какие свойства необходимо удалить
                    var deleteProps = vType.Properties.Except(balloonType.Properties).ToList();

                    // убираем и добавляем свойства у типа арматуры
                    foreach (var dp in deleteProps)
                    {
                        vType.DeleteProperty(dp.Id);
                    }
                    foreach (var np in newProps)
                    {
                        vType.AddProperty(np);
                    }

                    foreach (var prop in balloonType.Properties)
                    {
                        var editProp = vType.Properties.SingleOrDefault(p => p.Id == prop.Id);
                        editProp.Name = prop.Name;
                        editProp.Prefix = prop.Prefix;
                    }

                    db.GetCollection<BalloonType>(balloonTypeCollectionName).Update(balloonType);
                    var reactiveVt = balloonTypesReactiveList.Items.SingleOrDefault(vt => vt.Id == balloonType.Id);
                    if (reactiveVt!=null)
                    {
                        balloonTypesReactiveList.Replace(reactiveVt, balloonType);
                    }

                    // убираем и добавляем свойства у всей арматуры
                    foreach (var balloon in balloons)
                    {
                        foreach (var dp in deleteProps)
                        {
                            balloon.RemoveProperty(dp.Id);
                        }

                        foreach (var np in newProps)
                        {
                            balloon.Properties.Add(np);
                        }

                        foreach (var prop in balloonType.Properties)
                        {
                            var editProp = balloon.Properties.SingleOrDefault(p => p.Id == prop.Id);
                            editProp.Name = prop.Name;
                            editProp.Prefix = prop.Prefix;
                        }
                        db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .Update(balloon);
                        var rV = balloonReactiveList.Items.SingleOrDefault(v => v.Id == balloon.Id);
                        if (rV != null)
                        {
                            balloonReactiveList.Remove(rV);
                            balloonReactiveList.Add(balloon);
                        }
                    }

                }
                else
                {
                    db.GetCollection<BalloonType>(balloonTypeCollectionName).Insert(balloonType);
                    balloonTypesReactiveList.Add(balloonType);
                }
            }
        }

        public IObservable<IChangeSet<BalloonType>> ConnectToBalloonTypes()
        {
            return balloonTypesReactiveList.Connect();
        }

        public IEnumerable<Balloon> GetBalloons()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
               return db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .FindAll()
                        .ToList();
            }
        }

        public IEnumerable<BalloonType> GetBalloonTypes()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                return db.GetCollection<BalloonType>(balloonTypeCollectionName).FindAll().ToList();
            }
        }

        public void RemoveBalloon(Balloon balloon)
        {
            if (balloon == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                db.GetCollection<Balloon>(balloonCollectionName)
                    .Include(v => v.BalloonType)
                    .Delete(balloon.Id);
                balloonReactiveList.Remove(balloon);
            }
              
        }

        public void RemoveBalloonType(BalloonType balloonType)
        {
            if (balloonType == null) return;
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var balloons = db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType)
                        .FindAll()
                        .Where(v => v.BalloonType.Id == balloonType.Id);
                foreach (var balloon in balloons)
                {
                    db.GetCollection<Balloon>(balloonCollectionName)
                        .Include(v => v.BalloonType).Delete(balloon.Id);
                    balloonReactiveList.Remove(balloon);
                }
                db.GetCollection<BalloonType>(balloonTypeCollectionName).Delete(balloonType.Id);
                var reactiveVt = balloonTypesReactiveList.Items.SingleOrDefault(vt => vt.Id == balloonType.Id);
                if (reactiveVt != null)
                {
                    balloonTypesReactiveList.Remove(reactiveVt);
                }
            }
        }

        public IObservable<IChangeSet<Balloon>> ConnectToBalloons()
        {
            return balloonReactiveList.Connect();
        }
    }
}
