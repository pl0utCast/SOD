using MemBus;
using NLog;
using SOD.App.Benches.SODBench.Report;
using SOD.App.Commands;
using SOD.App.Commands.ModbusCommands;
using SOD.App.Messages;
using SOD.App.Messages.Commands;
using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.App.Testing.Standarts;
using SOD.App.Testing.Test;
using SOD.Core.Balloons;
using SOD.Core.Device.Modbus;
using SOD.Core.Infrastructure;
using SOD.Localization.Settings.DeviceAndSensors;
using SOD.LocalizationService;
using System.Drawing;
using UnitsNet;

namespace SOD.App.Benches.SODBench
{
    public class Bench : ITestBench
    {
        private const string settingsKey = "SODBenchSettings";
        private readonly ISettingsService _settingsService;
        private readonly ISensorService _sensorService;
        private readonly ITestingService _testingService;
        private readonly IBus _bus;
        private readonly IReportService _reportService;
        private readonly ILocalizationService _localizationService;
        private List<BenchSensor> sensors = new List<BenchSensor>();
        private ReportData reportData;
        private CancellationTokenSource cancellationTokenSource;
        private ITesting currentTest;
        private Timer registrationTimer;
        private ProgrammMethodicsConfig programmMethodicsConfig;
        private ModbusTcpDevice device;
        private ModbusCommandsFactory modbusCommandsFactory;
        private readonly List<Post> posts = new List<Post>();

        public Bench(ISettingsService settingsService,
                     ISensorService sensorService,
                     ITestingService testingService,
                     IBus bus,
                     IReportService reportService,
                     ILocalizationService localizationService,
                     IDeviceService deviceService)
        {
            _settingsService = settingsService;
            _sensorService = sensorService;
            _testingService = testingService;
            _bus = bus;
            _reportService = reportService;
            _localizationService = localizationService;
            Settings = settingsService?.GetSettings<Settings>(settingsKey, new Settings());

            device = deviceService.GetAllDevice().FirstOrDefault(d => d is ModbusTcpDevice) as ModbusTcpDevice;
            modbusCommandsFactory = new ModbusCommandsFactory(device, bus, _localizationService);

            posts.Add(new Post(1) { IsEnable = true, Name = "Post 1" });

            UpdatePosts();
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

            Standart = _testingService.GetAllStandarts().SingleOrDefault(s => s.Id == Settings.SelectedBalloon.StandartId);
            currentTest = new Testing.Test.Test(ProgrammMethodicsConfig.Name, reportData, _localizationService, Standart, testSettings.SetPressure, 3.0, new Pressure(1, UnitsNet.Units.PressureUnit.Bar), 10);
            if (currentTest != null)
            {
                cancellationTokenSource = new CancellationTokenSource();
                currentTest.Start(this);
                currentTest.StartCollectData();
            }

            var programm = _testingService.CreateProgrammMethodics(ProgrammMethodicsConfig, TestingBalloon, modbusCommandsFactory, reportData);

            Task.Run(async () =>
            {
                try
                {
                    foreach (var children in programm.Childrens)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        if (children is ICommand baseCommand)
                        {
                            _bus.Publish(new ExecuteTestCommand(baseCommand.CommandConfig, true));
                            await baseCommand.ExecuteAsync(cancellationTokenSource.Token, true);
                            _bus.Publish(new ExecuteTestCommand(baseCommand.CommandConfig, false));
                        }
                    }
                }
                catch (Exception e)
                {
                    currentTest?.Stop();
                    // посылаем контроллеру команду стоп
                    await device.WriteInt32(46, 2);
                    //logger.Error(e, $"Ошибка выполнения программной методики");
                }
            });
        }

        public void StartRegistration()
        {
            if (currentTest?.IsRun == false) return;
            var testSettings = Settings.SelectedTestSettings;
            registrationTimer?.Dispose();
            registrationTimer = new Timer(cb => StopRegistartion(), null,
                                                TimeSpan.FromSeconds((double)testSettings.Time),
                                                TimeSpan.FromSeconds((double)testSettings.Time));
            currentTest?.StartRegistration();
            _bus.Publish(new RegistrationMessage(RegistartionStatus.Start, testSettings.Time.Value));
        }

        public void StopRegistartion()
        {
            var testSettings = Settings.SelectedTestSettings;
            registrationTimer?.Dispose();
            currentTest?.StopRegistration();
            _bus.Publish(new RegistrationMessage(RegistartionStatus.End, testSettings.Time.Value));
        }

        public void StopTesting()
        {
            if (currentTest?.IsRun == false) return;
            registrationTimer?.Dispose();
            currentTest?.Stop();
            currentTest?.CalculateResult();
        }

        public void UpdatePosts()
        {
            posts.Clear();
            var post = new Post(1);
            post.IsEnable = true;

            post.Sensors.Clear();
            post.Sensors.AddRange(_sensorService.GetAllSensors()
                                                .Where(s => s.Id == Settings.SelectedTestSettings.PressureSensorId)
                                                .Select(s => new BenchSensor(s)));
            post.Sensors.AddRange(_sensorService.GetAllSensors()
                                                .Where(s => s.Id == Settings.SelectedTestSettings.TenzoSensorId)
                                                .Select(s => new BenchSensor(s)));
            posts.Add(post);
        }

        public void UpdateReport(Bitmap chart = null)
        {
            Task.Run(async () =>
            {
                if (TestingBalloon != null)
                {
                    currentTest?.FillReport(chart);
                    reportData.Fill(Settings.BalloonProperties);
                    reportData.Fill(TestingBalloon);
                    reportData.Fill(Settings.Parameters);
                    var report = await _reportService.CreateReportAsync(reportData, Settings.ReportPath);

                    foreach (var prop in Settings.BalloonProperties)
                    {
                        report.Properties.Add(prop.Prefix, prop.Value.ToString());
                    }
                    if (TestingBalloon.BalloonType != null)
                        report.Properties.Add("balloon_type", TestingBalloon.BalloonType.ToString());
                    report.Properties.Add("balloon_volume", TestingBalloon.BalloonVolume.ToString());
                }
            });
        }

        public IEnumerable<BenchSensor> Sensors => posts.First().Sensors;
        public Settings Settings { get; private set; } = new Settings();

        private Balloon testingBalloon;
        public Balloon TestingBalloon
        {
            get => testingBalloon;
            set
            {
                if (value != null && !value.Equals(testingBalloon))
                {
                    reportData = new ReportData(_sensorService, this);
                    reportData?.Fill(value);
                }
                testingBalloon = value;
            }
        }

        public ProgrammMethodicsConfig ProgrammMethodicsConfig
        {
            get
            {
                return programmMethodicsConfig;
            }
            set
            {
                programmMethodicsConfig = value;
            }
        }

        IBenchSettings ITestBench.Settings => Settings;
        public IEnumerable<IPost> Posts => posts;
        public ITesting CurrentTest => currentTest;
        public LuaStandart Standart { get; set; }
    }
}
