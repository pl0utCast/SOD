using SOD.Core.Valves;
using SOD.App.Mediums;
using System;
using System.Collections.Generic;
using System.Text;
using NLua;
using NLog;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace SOD.App.Testing.Standarts
{
    
    public class LuaStandart : IStandart
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public LuaStandart()
        {
            
        }

        public double GetWaitTime(params object[] parameters)
        {
            return (double)Call("get_wait_time", parameters);
        }

        public double GetRegistrationTime(params object[] parameters)
        {
            return (double)Call("get_registartion_time", parameters);
        }

        private object Call(string functionName, params object[] parameters)
        {
            try
            {
                using (var state = new Lua())
                {
                    state.LoadCLRPackage();
                    state.DoFile(Path.Combine(Directory.GetCurrentDirectory(), Const.SCRIPTS_PATH, Script));
                    var func = state[functionName] as LuaFunction;
                    if (func != null)
                    {
                        return func.Call(parameters)[0];
                    }

                }
            }
            catch (Exception e)
            {
                logger.Warn(e, $"Ошибка выполнения функции {functionName} стандарта {Name}");
            }
            return null;
        }

        public StandartResult Calculate(params object[] parameters)
        {
            return (StandartResult)Call("calculate", parameters);
        }

        public string Script { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> ValveTypesId { get; set; } = new List<int>();
        public bool IsLeakage { get; set; }
        public IList<TestType> SupportTests { get; set; } = new List<TestType>();
    }
}
