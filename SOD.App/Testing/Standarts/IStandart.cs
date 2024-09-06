using SOD.Core.Valves;
using SOD.App.Mediums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Standarts
{
    public interface IStandart
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsLeakage { get; }
        public StandartResult Calculate(params object[] parameters);
        public double GetWaitTime(params object[] parameters);
        public double GetRegistrationTime(params object[] parameters);
        public IList<TestType> SupportTests { get; }
        public List<int> ValveTypesId { get; set; }
    }
}
