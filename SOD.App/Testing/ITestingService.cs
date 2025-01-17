using DynamicData;
using SOD.App.Commands;
using SOD.App.Testing.Programms;
using SOD.App.Testing.Standarts;
using SOD.Core.Balloons;
using SOD.Core.Reports;

namespace SOD.App.Testing
{
    public interface ITestingService
    {
        public void AddOrUpdate(ProgrammMethodicsConfig config);
        public void Remove(ProgrammMethodicsConfig config);
        public ProgrammMethodics CreateProgrammMethodics(ProgrammMethodicsConfig config, Balloon valve, CommandsFactory commandsFactory, BaseReportData reportData);
        public IEnumerable<ProgrammMethodicsConfig> GetAllProgrammMethodicsConfig();

        public void AddOrUpdate(LuaStandart standart);
        public void Remove(LuaStandart standart);
        public IEnumerable<LuaStandart> GetAllStandarts();

        public IObservable<IChangeSet<ProgrammMethodicsConfig>> ConnectToProgrammMethodics();
        public IObservable<IChangeSet<LuaStandart>> ConnectToStandarts();
    }
}
