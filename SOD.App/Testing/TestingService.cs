using DynamicData;
using LiteDB;
using MemBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOD.Core;
using SOD.Core.Infrastructure;
using SOD.Core.Reports;
using SOD.Core.Valves;
using SOD.App.Commands;
using SOD.App.Interfaces;
using SOD.App.Testing.Programms;
using SOD.App.Testing.Standarts;
using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitsNet.Serialization.JsonNet;
using JsonNet.PrivateSettersContractResolvers;

namespace SOD.App.Testing
{
    public class TestingService : ITestingService
    {
        private readonly SourceList<ProgrammMethodicsConfig> programmMethodicsConfigReactiveList = new SourceList<ProgrammMethodicsConfig>();
        private readonly SourceList<LuaStandart> standartsReactiveList = new SourceList<LuaStandart>();
        private readonly ProgramMethodicsBuilder methodicsBuilder;

        private string dbProgrammPath = Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder, "ProgrammMethodics.db");
        private string dbStandartPath = Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder, "Standarts.db");
        private Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        

        public TestingService(IValveService valveService, IBus bus, ILocalizationService localizationService)
        {
            methodicsBuilder = new ProgramMethodicsBuilder(valveService, GetAllStandarts(), bus, localizationService);

            serializer.Converters.Add(new UnitsNetIQuantityJsonConverter());
            serializer.Converters.Add(new KeyValuePairTypeJsonConverter());
            serializer.Converters.Add(new TestConfigJsonConverter());
            serializer.ContractResolver = new PrivateSetterContractResolver();
            BsonMapper.Global.RegisterType<KeyValuePair<Type, object>>(
                serialize: kv =>
                {
                    return JsonConvert.SerializeObject(JObject.FromObject(kv, serializer));
                },
                deserialize: bson =>
                {
                    return JsonConvert.DeserializeObject<JObject>(bson.AsString).ToObject<KeyValuePair<Type, object>>(serializer);
                });
            BsonMapper.Global.RegisterType<IBranch<ITestConfig>>(
                serialize: tc =>
                {
                    var s = JsonConvert.SerializeObject(JObject.FromObject(tc, serializer));
                    return s;
                },
                deserialize: bson =>
                {
                    return JsonConvert.DeserializeObject<JObject>(bson.AsString).ToObject<IBranch<ITestConfig>>(serializer);
                });

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder));
            }
            programmMethodicsConfigReactiveList.AddRange(GetAllProgrammMethodicsConfig());
            standartsReactiveList.AddRange(GetAllStandarts());

        }
        public ProgrammMethodics CreateProgrammMethodics(ProgrammMethodicsConfig config, Valve valve, CommandsFactory commandsFactory, BaseReportData reportData)
        {
            return methodicsBuilder.Build(config, valve, commandsFactory, reportData);
        }

        public void AddOrUpdate(ProgrammMethodicsConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            using (var db = new LiteDatabase(dbProgrammPath, BsonMapper.Global))
            {
                if (db.GetCollection<ProgrammMethodicsConfig>().Exists(c => c.Id == config.Id))
                {
                    db.GetCollection<ProgrammMethodicsConfig>().Update(config);
                    programmMethodicsConfigReactiveList.Edit(list => list.ReplaceOrAdd(config, config));
                }
                else
                {
                    db.GetCollection<ProgrammMethodicsConfig>().Insert(config);
                    programmMethodicsConfigReactiveList.Add(config);
                }
            }
        }

        public IEnumerable<ProgrammMethodicsConfig> GetAllProgrammMethodicsConfig()
        {
            using (var db = new LiteDatabase(dbProgrammPath, BsonMapper.Global))
            {
                return db.GetCollection<ProgrammMethodicsConfig>().FindAll().ToList();
            }
        }

        public IObservable<IChangeSet<ProgrammMethodicsConfig>> ConnectToProgrammMethodics()
        {
            return programmMethodicsConfigReactiveList.Connect();
        }


        public void AddOrUpdate(LuaStandart standart)
        {
            using (var db = new LiteDatabase(dbStandartPath, BsonMapper.Global))
            {
                if (db.GetCollection<LuaStandart>().Exists(s => s.Id == s.Id) && standart.Id!=0)
                {
                    db.GetCollection<LuaStandart>().Update(standart);
                    standartsReactiveList.Edit(list => list.ReplaceOrAdd(standart, standart));
                }
                else
                {
                    db.GetCollection<LuaStandart>().Insert(standart);
                    standartsReactiveList.Add(standart);
                }
            }
        }

        public IEnumerable<LuaStandart> GetAllStandarts()
        {
			return new List<LuaStandart>
			{
				new() {Id=0, Name = "ГОСТ ISO 11439" },
                new() {Id=1, Name = "ISO 11119-3"},
                new() {Id=2, Name = "ГОСТ 33986-2016"},
                new() {Id=3, Name = "ГОСТ Р 51753-2001"}
			};
            //using (var db = new LiteDatabase(dbStandartPath, BsonMapper.Global))
            //{
            //    return db.GetCollection<LuaStandart>().FindAll().ToList();
            //}
        }

        public IObservable<IChangeSet<LuaStandart>> ConnectToStandarts()
        {
            return standartsReactiveList.Connect();
        }

        public void Remove(LuaStandart standart)
        {
            using (var db = new LiteDatabase(dbStandartPath, BsonMapper.Global))
            {
                if (db.GetCollection<LuaStandart>().Exists(s => s.Id == s.Id))
                {
                    db.GetCollection<LuaStandart>().Delete(standart.Id);
                    standartsReactiveList.Remove(standart);
                }
            }
        }

        public void Remove(ProgrammMethodicsConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            using (var db = new LiteDatabase(dbProgrammPath, BsonMapper.Global))
            {
                db.GetCollection<ProgrammMethodicsConfig>().Delete(config.Id);
                programmMethodicsConfigReactiveList.Remove(config);
            }
        }

        public IObservable<ProgrammMethodicsConfig> ProgrammMethodic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
