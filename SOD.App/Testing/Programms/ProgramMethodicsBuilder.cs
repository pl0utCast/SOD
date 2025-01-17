using MemBus;
using SOD.App.Commands;
using SOD.App.Testing.Standarts;
using SOD.Core.Infrastructure;
using SOD.Core.Reports;
using SOD.Core.Balloons;
using SOD.LocalizationService;

namespace SOD.App.Testing.Programms
{
    public class ProgramMethodicsBuilder
    {
        private readonly IBalloonService _balloonService;
        private readonly IEnumerable<IStandart> _standarts;
        private readonly IBus _bus;
        private readonly ILocalizationService localizationService;

        public ProgramMethodicsBuilder(IBalloonService balloonService, IEnumerable<IStandart> standarts, IBus bus, ILocalizationService localizationService)
        {
            _balloonService = balloonService;
            _standarts = standarts;
            _bus = bus;
            this.localizationService = localizationService;
        }
        public ProgrammMethodics Build(ProgrammMethodicsConfig config, Balloon balloon, CommandsFactory commandsFactory, BaseReportData reportData)
        {
            var programmMethodics = new ProgrammMethodics();
            if (config == null) throw new ArgumentNullException(nameof(config));

            programmMethodics.Id = config.Id;
            programmMethodics.CreatedDate = config.CreatedDate;
            programmMethodics.Name = config.Name;
            // делаем запрос на тип арматуры и получаем его
            programmMethodics.BalloonType = _balloonService.GetBalloonTypes().SingleOrDefault(at => at.Id == config.BalloonTypeId);
            // создаём программы испытания
            foreach (var children in config.Childrens)
            {
                if (children is TestProgrammConfig testProgrammConfig)
                {
                    var testProgramm = new TestProgramm() { Id = testProgrammConfig.Id };
                    if (testProgrammConfig.StandartId!=null)
                        testProgramm.Standart = _standarts.SingleOrDefault(s => s.Id == testProgrammConfig.StandartId);
					testProgramm.Test = new Test.Test(reportData, localizationService, testProgramm.Standart, testProgrammConfig.Parameters.Select(kv => kv.Value).ToArray());
					switch (testProgrammConfig.TestType)
                    {
                        //case TestType.Strength:
                        //    testProgramm.Test = new Strength.Test(testProgrammConfig.Name, reportData, localizationService, testProgramm.Standart, testProgrammConfig.Parameters.Select(kv => kv.Value).ToArray());
                        //    break;
                        //case TestType.Leakage:
                        //    testProgramm.Test = new Leakage.Test(testProgrammConfig.Name, reportData, localizationService, testProgramm.Standart, testProgrammConfig.Parameters.Select(kv => kv.Value).ToArray());
                        //    break;
                        default:
                            break;
                    }
                    foreach (var commandChildren in children.Childrens)
                    {
                        if (commandChildren is CommandConfig cc)
                        {
                            testProgramm.Childrens.Add(commandsFactory.Create(cc, balloon));
                        }
                    }
                    programmMethodics.Childrens.Add(testProgramm);
                }
                if (children is CommandConfig commandConfig)
                {
                    programmMethodics.Childrens.Add(commandsFactory.Create(commandConfig, balloon));
                }
            }
            return programmMethodics;
        }
    }
}
