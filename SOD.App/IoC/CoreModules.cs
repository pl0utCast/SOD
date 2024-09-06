using MemBus;
using MemBus.Configurators;
using Ninject.Modules;
using SOD.Core.Configuration;
using SOD.Core.Device;
using SOD.Core.Infrastructure;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.Core.Settings;
using SOD.Core.Valves;
using SOD.App.Benches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.IoC
{
    public class CoreModules : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfigService>().To<ConfigService>().InSingletonScope();
            Bind<IDeviceService>().To<DeviceService>().InSingletonScope();
            Bind<ISensorService>().To<SensorService>().InSingletonScope();
            Bind<ISettingsService>().To<LocalSettingsService>().InSingletonScope();
            Bind<IValveService>().To<ValveSevice>().InSingletonScope();
            Bind<IBus>().ToConstant(MemBus.BusSetup.StartWith<Conservative>().Construct()).InSingletonScope();
            Bind<ITestBenchService>().To<TestBenchService>().InSingletonScope();
            Bind<IReportService>().To<ReportService>().InSingletonScope();
        }
    }
}
