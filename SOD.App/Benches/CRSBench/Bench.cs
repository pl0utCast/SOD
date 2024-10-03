using MemBus;
using SOD.App.Benches.CRSBench.Report;
using SOD.App.Messages.Commands;
using SOD.App.Testing;
using SOD.App.Testing.Standarts;
using SOD.Core.Infrastructure;
using SOD.LocalizationService;
using System.Drawing;

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
		private List<BenchSensor> sensors = new List<BenchSensor>();
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
			currentTest = new Testing.Test.Test(reportData, _localizationService, Standart, testSettings.SetPressure);
			if (currentTest != null)
			{
				cancellationTokenSource = new CancellationTokenSource();
				currentTest.Start(this);
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
			sensors.Clear();
			sensors.AddRange(_sensorService.GetAllSensors()
										   .Where(s => s.Id == Settings.SelectedTestSettings.PressureSensorId)
										   .Select(s => new BenchSensor(s)));
		}

		public void UpdateReport(Bitmap chart = null)
		{
			Task.Run(async () =>
			{
				if (TestingBalloon != null)
				{
					currentTest?.FillReport(chart);
					Settings.BalloonProperties.Insert(0,new Core.Balloons.Properties.BalloonProperty() { Name = _localizationService["Testing.CRSBench.TestSettings.BalloonType"], Value = Settings.SelectedBalloon.Name });
					Settings.BalloonProperties.Insert(1,new Core.Balloons.Properties.BalloonProperty() { Name = _localizationService["Testing.CRSBench.TestSettings.BalloonValue"], Value = Settings.SelectedBalloon.BalloonVolume.ToString() });
					reportData.Fill(Settings.BalloonProperties);
					reportData.Fill(Settings.Parameters);
					var report = await _reportService.CreateReportAsync(reportData, Settings.ReportPath);

					//foreach (var prop in TestingBalloon.Properties)
					//{
					//	report.Properties.Add(prop.Prefix, prop.Value.ToString());
					//}
					//report.Properties.Add("balloon_type", TestingBalloon.BalloonType.ToString());
					//report.Properties.Add("balloon_volume", TestingBalloon.BalloonVolume.ToString());
				}
			});
		}

		public IEnumerable<BenchSensor> Sensors => sensors;
		public Settings Settings { get; private set; } = new Settings();

		private Core.Balloons.Balloon testingBalloon;
		public Core.Balloons.Balloon TestingBalloon
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

		IBenchSettings ITestBench.Settings => Settings;
		public ITesting CurrentTest => currentTest;
		public LuaStandart Standart { get; set; }
	}
}
