using MemBus;
using Ninject;
using SOD.Core.Infrastructure;
using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SOD.LocalizationService;

namespace SOD.App.Benches
{
    public class TestBenchService : ITestBenchService
    {
        private const string SETTINGS_KEY = "TestBenchService";
        private readonly IDeviceService _deviceService;
        private readonly ISettingsService _settingsService;
        private readonly ITestingService _testingService;
        private readonly ISensorService _sensorService;
        private readonly IBus _bus;
        private readonly IReportService _reportService;
        private readonly IKernel kernel;
        private readonly ILocalizationService localizationService;
        private SODBench.Bench sodBench;
        private ITestBench tempTestingBench;
        public TestBenchService(IDeviceService deviceService,
                                ISettingsService settingsService,
                                IBus bus,
                                ITestingService testingService,
                                ISensorService sensorService,
                                IReportService reportService,
                                IKernel kernel,
                                ILocalizationService localizationService)
        {
            _sensorService = sensorService;
            _deviceService = deviceService;
            _settingsService = settingsService;
            _bus = bus;
            _testingService = testingService;
            _reportService = reportService;
            this.kernel = kernel;
            this.localizationService = localizationService;
            LoadSettings();
		}

        public ITestBench GetTestBench()
        {
            switch (Settings.BenchType)
            {
                case BenchesType.SOD:
                    if (sodBench == null)
                    {
                        sodBench = new SODBench.Bench(_settingsService, _sensorService, _testingService, _bus, _reportService, localizationService);
                    }
                    return sodBench;
                default: return null;
            }
        }

        public Settings Settings { get; set; }

        public void LoadSettings()
        {
            Settings = _settingsService.GetSettings<Settings>(SETTINGS_KEY, new Settings());
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(SETTINGS_KEY, Settings);
        }
    }
}
