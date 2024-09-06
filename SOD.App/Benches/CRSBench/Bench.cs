using MemBus;
using SOD.App.Benches.CRSBench.Report;
using SOD.App.Messages.Commands;
using SOD.App.Testing;
using SOD.App.Testing.Standarts;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor.LeakageSensor.Impulse;
using SOD.Core.Valves;
using SOD.LocalizationService;
using System.Drawing;
using UnitsNet;

namespace SOD.App.Benches.CRSBench
{
    public class Bench : ITestBench
    {
        private const string settingsKey = "CRSBenchSettings";
        private readonly ISettingsService _settingsService;
        private readonly ISensorService _sensorService;
        private readonly ITestingService _testingService;
        private readonly IBus _bus;
        private readonly IReportService _reportService;
        private readonly ILocalizationService _localizationService;
        private List<Post> posts = new List<Post>();
        private ReportData reportData;
        private CancellationTokenSource cancellationTokenSource;
        private ITesting currentTest;
        private Timer registrationTimer;
        public Bench(ISettingsService settingsService,
                     ISensorService sensorService,
                     ITestingService testingService,
                     IBus bus,
                     IReportService reportService,
                     ILocalizationService localizationService)
        {
            _settingsService = settingsService;
            _sensorService = sensorService;
            _testingService = testingService;
            _bus = bus;
            _reportService = reportService;
            _localizationService = localizationService;
            Settings = settingsService?.GetSettings<Settings>(settingsKey, new Settings());
            
            UpdateReport();

            bus.Subscribe<App.Messages.Reports.ReportSaveMessage>(m =>
            {
                reportData = new ReportData(sensorService, this); 
                currentTest?.Result.Clear();
                UpdateReport();
            });

            bus.Subscribe<App.Messages.Reports.CreateNewReportMessage>(m =>
            {
                reportData = new ReportData(sensorService, this);
                currentTest?.Result.Clear();
                UpdateReport();
            });
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(settingsKey, Settings);
        }

        public void StartTesting()
        {
            if (currentTest?.IsRun == true) return;
            var testSettings = Settings.SelectedTestSettings;
            Standart = _testingService.GetAllStandarts().SingleOrDefault(s => s.Id == testSettings.StandartId);
            switch (Settings.SelectedTestSettings.Type)
            {
                case TestType.Strength:
                    currentTest = new Testing.Strength.Test(testSettings.LocalName, reportData, _localizationService, Standart, Testing.Strength.ControlType.Manual);
                    break;
                case TestType.Leakage:
                    currentTest = new Testing.Leakage.Test(testSettings.LocalName, reportData, _localizationService, Standart);
                    break;
                case TestType.Functional:
                    var setPressure = Settings.SelectedTestSettings.SetPressure;
                    if (Settings.SelectedTestSettings.UseValveSetPressure)
                    {
                        setPressure = (Pressure)TestingValve?.GetValveProperty("set_pressure")?.Value;
                    }
                    currentTest = new Testing.Funcional.Test(testSettings.LocalName, reportData, _localizationService, Standart, setPressure, 3.0, Settings.FuncionalityTestSensetive, Settings.FuncionalityTestWindowSize );
                    break;
                default:
                    break;
            }
            if (currentTest != null)
            {
                cancellationTokenSource = new CancellationTokenSource();
                currentTest.Start( this, testSettings.MediumType);
                currentTest.StartCollectData();
            }
        }

        public void StartRegistration()
        {
            if (currentTest?.IsRun == false) return;
            var testSettings = Settings.SelectedTestSettings;
            registrationTimer?.Dispose();
            registrationTimer = new Timer(cb => StopRegistartion(), null, 
                TimeSpan.FromSeconds((double)testSettings.Time), 
                TimeSpan.FromSeconds((double)testSettings.Time));
            currentTest.StartRegistration();
            if (Settings.SelectedTestSettings.Type == TestType.Leakage) //перезапуск таймера измерения расходомеров
            {
                var sensor = _sensorService.GetSensor((int)Settings.SelectedTestSettings.LeakageSensorId);
                if (sensor is ImpulseSensor impulseSensor)
                {
                    impulseSensor.Reset();
                }
            }
            _bus.Publish(new RegistrationMessage(RegistartionStatus.Start, testSettings.Time.Value));
        }

        public void StopRegistartion()
        {
            var testSettings = Settings.SelectedTestSettings;
            registrationTimer?.Dispose();
            currentTest.StopRegistration();
            _bus.Publish(new RegistrationMessage(RegistartionStatus.End, testSettings.Time.Value));
        }

        public void StopTesting()
        {
            if (currentTest?.IsRun == false) return;
            registrationTimer?.Dispose();
            currentTest?.Stop();
            currentTest?.CalculateResult();
        }

        public void UpdateReport(Bitmap chart=null)
        {
            Task.Run(async () =>
            {
                if (TestingValve!=null)
                {
                    currentTest?.FillReport(chart);
                    reportData.Fill(TestingValve);
                    reportData.Fill(Settings.Parameters);
                    var report = await _reportService.CreateReportAsync(reportData, Settings.ReportPath);
                    foreach (var prop in TestingValve.Properties)
                    {
                        report.Properties.Add(prop.Prefix, prop.Value.ToString());
                    }
                    report.Properties.Add("valve_name", TestingValve.Name);
                    report.Properties.Add("valve_type", TestingValve.ValveType.Name);
                }
            }); 
        }

        public BenchesType Type => BenchesType.CRS;

        public IEnumerable<IPost> Posts => posts;

        public Settings Settings { get; private set; } = new Settings();

        private Valve testingVale;
        public Valve TestingValve
        { 
            get => testingVale; 
            set
            {
                if (value != null && !value.Equals(testingVale))
                {
                    reportData = new ReportData(_sensorService, this);
                    reportData.Fill(value);
                }
                testingVale = value;
            }
        }

        IBenchSettings ITestBench.Settings => Settings;
        public ITesting CurrentTest => currentTest;
        public LuaStandart Standart { get; set; }
    }
}
