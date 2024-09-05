using DynamicData;
using SOD.Core.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface IValveService
    {
        IEnumerable<ValveType> GetValveTypes();
        void AddOrUpdateValveType(ValveType valveType);
        void RemoveValveType(ValveType valveType);

        void AddOrUpdateValve(Valve valve);
        void RemoveValve(Valve valve);
        IEnumerable<Valve> GetValves();

        IObservable<IChangeSet<ValveType>> ConnectToValveTypes();
        IObservable<IChangeSet<Valve>> ConnectToValves();
    }
}
