using Ninject;
using SOD.App.Testing;
using SOD.Core.Infrastructure;
using SOD.LocalizationService;

namespace SOD.App
{
    public class Startup
    {

        public Startup()
        {
            Kernel = new StandardKernel(new IoC.CoreModules());
            Kernel.Bind<ILocalizationService>()
                .To<LocalizationService<SOD.Localization.Localization>>()
                .InSingletonScope();
            Kernel.Bind<ITestingService>().To<TestingService>().InSingletonScope();
            // run all device
            foreach (var device in Kernel.Get<IDeviceService>().GetAllDevice())
            {
                device.Connenct();
            }
        }

        public IKernel Kernel { get; private set; }
    }
}
