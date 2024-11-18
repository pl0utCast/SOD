using DynamicData.Binding;
using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Benches;
using SOD.Core;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.ViewModels.Extensions;
using SOD.ViewModels.Testing.SODBench.Sensors;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using SensorViewModel = SOD.ViewModels.Testing.SODBench.Sensors.SensorViewModel;

namespace SOD.ViewModels.Testing.SODBench
{
	public class DefaultTestingViewModel : ReactiveObject, IActivatableViewModel
	{
		private App.Benches.SODBench.Bench _bench;
		private IDisposable timer;
		private bool isAddTestToReport = false;
		private int exposureCounter = 0;
		public DefaultTestingViewModel(INavigationService navigationService,
										ITestBenchService testBenchService,
										IBus bus,
										IDialogService dialogService,
										ISensorService sensorService,
										ILocalizationService localizationService)
		{
			_bench = (App.Benches.SODBench.Bench)testBenchService.GetTestBench();
			PressureChart = new PressureChartViewModel(localizationService, _bench);
			if (_bench.Settings.SelectedTestSettings != null /*&& _bench.TestingValve!=null*/) IsSelectedTest = true;
			ExposureTime = "00:00:00";
			TemperatureSensors = new TemperatureSensorsViewModel(sensorService, _bench);
			UpdateChart();
			UpdateSensors();
			InfoMessage = localizationService["Testing.SODBench.Step1"];

			bus.Subscribe<App.Benches.SODBench.Messages.SelectedTestMessage>(m =>
			{
				IsTestResultFill = false;
				IsSelectedTest = true;
				UpdateChart();
				UpdateSensors();
				InfoMessage = localizationService["Testing.SODBench.Step2"];
			});

			StartTest = ReactiveCommand.Create(() =>
			{
				if (IsRunTest)
				{
					bus.Publish(new App.Messages.ProgrammMethodicsStatus(App.Messages.ProgrammStatus.Stop));
					if (IsExposure)
					{
						IsExposure = false;
						timer.Dispose();
					}
					_bench.StopTesting();
					if (exposureCounter > 0)
					{
						IsTestResultFill = true;
						InfoMessage = localizationService["Testing.SODBench.Step6_2"];
					}
					else
					{
						IsTestResultFill = false;
						InfoMessage = localizationService["Testing.SODBench.Step2"];
					}
					isAddTestToReport = IsTestResultFill;
					PressureChart.StopChart();
					var chart = PressureChart.PressureSeries.FirstOrDefault().DataSeries.ParentSurface.ExportToBitmapSource().GetBitmap();
				}
				else
				{
					InfoMessage = localizationService["Testing.SODBench.Step3"];
					exposureCounter = 0;
					ExposureTime = "00:00:00";
					bus.Publish(new App.Messages.ProgrammMethodicsStatus(App.Messages.ProgrammStatus.Run));
					_bench.StartTesting();
					UpdateChart();
					PressureChart.StartChart();
				}
				IsRunTest = !IsRunTest;
			}, this.WhenAnyValue(x => x.IsExposure, x => x.IsSelectedTest,
				(isExposure, isSelectedTest) => !isExposure && isSelectedTest));

			Exposure = ReactiveCommand.Create(() =>
			{
				if (!IsExposure && exposureCounter < 3)
				{
					InfoMessage = localizationService["Testing.SODBench.Step4"];
					exposureCounter++;
					IsExposure = true;
					_bench.StartRegistration();
				}
				else if (IsExposure && exposureCounter <= 3)
				{
					InfoMessage = localizationService["Testing.SODBench.Step5"];
					IsExposure = false;
					_bench.StopRegistartion();
				}
			}, this.WhenAnyValue(x => x.IsRunTest));
			GoParameters = ReactiveCommand.Create(() =>
			{
				navigationService.NavigateTo("SODTestParameters");
			}, this.WhenAnyValue(x => x.IsRunTest).Select(isRun => !isRun));


			var canResult = this.WhenAnyValue(x => x.IsRunTest, x => x.IsTestResultFill,
				(isRunTest, isTestResultFill) => !isRunTest && isTestResultFill);

			Result = ReactiveCommand.CreateFromTask(async () =>
			{
				var vm = new Dialogs.TestResultViewModel(((App.Testing.Test.Test)_bench.CurrentTest).TestResult.PostResults.FirstOrDefault(), localizationService, dialogService, Observable.Return(isAddTestToReport));

				var result = await dialogService.ShowDialogAsync("SODBenchTestResult", vm);
				if (result != null)
				{
					isAddTestToReport = false;
					_bench.UpdateReport(PressureChart.PressureSeries.FirstOrDefault().DataSeries.ParentSurface.ExportToBitmapSource().GetBitmap());
				}
				InfoMessage = localizationService["Testing.SODBench.Step2"];
			}, canResult);

			this.WhenActivated(dis =>
			{
				bus.Subscribe<App.Messages.Commands.RegistrationMessage>(m =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						timer?.Dispose();
						if (m.Status == App.Messages.Commands.RegistartionStatus.Start)
						{
							var exposureTime = TimeSpan.FromSeconds(_bench.Settings.SelectedTestSettings.Time.Value);
							timer = Observable.Timer(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000))
									  .Subscribe(t => ExposureTime = (exposureTime - TimeSpan.FromSeconds(t + 1)).ToString("hh\\:mm\\:ss"))
									  .DisposeWith(dis);
						}
						else if (m.Status == App.Messages.Commands.RegistartionStatus.End)
						{
							IsExposure = false;
						}
						PressureChart.SetAnnotation();
					});
				})
				.DisposeWith(dis);

				bus.Subscribe<App.Messages.Reports.ReportSaveMessage>(m =>
				{
					IsTestResultFill = false;
				}).DisposeWith(dis);
				bus.Subscribe<App.Messages.Reports.CreateNewReportMessage>(m =>
				{
					IsTestResultFill = false;
				}).DisposeWith(dis);
			});
		}

		private void UpdateChart()
		{
			PressureChart.ClearChart();
			foreach (var pSensor in _bench.Sensors.Where(s => s.Sensor is IPressureSensor).Select(s => s.Sensor))
			{
				PressureChart.SetPressureSensor((IPressureSensor)pSensor);
			}
		}

		private void UpdateSensors()
		{
			Sensors.Clear();
			Sensors.AddRange(_bench.Sensors.Where(s => s.Sensor is IPressureSensor)
										   .Select(s => new PressureSensorViewModel((IPressureSensor)s.Sensor, _bench.Settings.PressureUnit)));
			Sensors.AddRange(_bench.Sensors.Where(s => s.Sensor is ITensoSensor)
										   .Select(s => new TensoSensorViewModel((ITensoSensor)s.Sensor, _bench.Settings.TensoUnit)));
		}

		[Reactive]
		public bool IsTestResultFill { get; set; }
		[Reactive]
		public bool IsExposure { get; set; }
		[Reactive]
		public bool IsSelectedTest { get; set; }
		[Reactive]
		public bool IsRunTest { get; set; }
		[Reactive]
		public string ExposureTime { get; set; }
		[Reactive]
		public string InfoMessage { get; set; }
		public ObservableCollectionExtended<SensorViewModel> Sensors { get; set; } = new ObservableCollectionExtended<SensorViewModel>();
		public PressureChartViewModel PressureChart { get; set; }
		public TemperatureSensorsViewModel TemperatureSensors { get; set; }
		public ReactiveCommand<Unit, Unit> GoParameters { get; set; }
		public ReactiveCommand<Unit, Unit> StartTest { get; set; }
		public ReactiveCommand<Unit, Unit> Exposure { get; set; }
		public ReactiveCommand<Unit, Unit> Result { get; set; }

		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
