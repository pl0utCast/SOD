using DynamicData.Binding;
using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals;
using SOD.App.Benches;
using SOD.App.Benches.SODBench;
using SOD.App.Messages;
using SOD.App.Testing.Programms;
using SOD.Core;
using SOD.Core.Device.Modbus;
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
        private ILocalizationService _localizationService;
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
            _localizationService = localizationService;
            Chart = new ChartViewModel(localizationService, _bench, bus);
            if (_bench.Settings.SelectedTestSettings != null /*&& _bench.TestingValve!=null*/) IsSelectedTest = true;
            ExposureTime = "00:00:00";
            TemperatureSensors = new TemperatureSensorsViewModel(sensorService, _bench);
            UpdateChart();
            UpdateSensors();
            //InfoMessage = localizationService["Testing.SODBench.Step1"];

            bus.Subscribe<App.Benches.SODBench.Messages.SelectedTestMessage>(m =>
            {
                IsTestResultFill = false;
                IsSelectedTest = true;
                UpdateChart();
                UpdateSensors();
            });

			StartTest = ReactiveCommand.CreateFromTask(async () =>
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
					}
					else
					{
						IsTestResultFill = false;
					}
					isAddTestToReport = IsTestResultFill;
					Chart.StopChart();
					var chart = Chart.Series.FirstOrDefault().DataSeries.ParentSurface.ExportToBitmapSource().GetBitmap();
				}
				else
				{
					exposureCounter = 0;
					ExposureTime = "00:00:00";
					bus.Publish(new App.Messages.ProgrammMethodicsStatus(App.Messages.ProgrammStatus.Run));
					_bench.StartTesting();
                    UpdateChart();
                    Chart.StartChart();
                    WaitingDefBalloon();
                }
                IsRunTest = !IsRunTest;
            }, this.WhenAnyValue(x => x.IsExposure, x => x.IsSelectedTest, x => x.ProgrammMethodicsConfig,
                    (isExposure, isSelectedTest, selectedProgrammMethodics) =>
                        !isExposure && isSelectedTest && ProgrammMethodicsConfig != null));

            Exposure = ReactiveCommand.Create(() =>
            {
                if (!IsExposure && exposureCounter < 3)
                {
                    exposureCounter++;
                    IsExposure = true;
                    Chart.SetExposureLine();
                    _bench.StartRegistration();
                }
                else if (IsExposure && exposureCounter <= 3)
                {
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
                    _bench.UpdateReport(Chart.Series.FirstOrDefault().DataSeries.ParentSurface.ExportToBitmapSource().GetBitmap());
                }
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
                        Chart.SetExposureLine();
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

                bus.Subscribe<SelectProgrammMethodicsConfigMessage>(m =>
                {
                    ProgrammMethodicsConfig = m.ProgrammMethodicsConfig;
                })
                .DisposeWith(dis);
            });
        }

        private void UpdateChart()
        {
            //PressureChart.ClearChart();
            //foreach (var pSensor in _bench.Sensors.Where(s => s.Sensor is IPressureSensor).Select(s => s.Sensor))
            //{
            //	//PressureChart.SetPressureSensor((IPressureSensor)pSensor);
            //}
            //         foreach (var tSensor in _bench.Sensors.Where(s => s.Sensor is ITenzoSensor).Select(s => s.Sensor))
            //         {
            //             PressureChart.SetPressureSensor((IPressureSensor)pSensor);
            //         }
            Chart.ClearChart();

            var pressureSensors = _bench.Sensors
            .Select(s => s.Sensor)
            .OfType<IPressureSensor>();

            var tenzoSensors = _bench.Sensors
                .Select(s => s.Sensor)
                .OfType<ITenzoSensor>();

            foreach (var pressureSensor in pressureSensors)
            {
                foreach (var tenzoSensor in tenzoSensors)
                {
                    Chart.SetSensors((IPressureSensor)pressureSensor, (ITenzoSensor)tenzoSensor);
                }
            }

        }

        private void UpdateSensors()
        {
            Sensors.Clear();
            Sensors.AddRange(_bench.Sensors.Where(s => s.Sensor is IPressureSensor)
                                           .Select(s => new PressureSensorViewModel((IPressureSensor)s.Sensor, _bench.Settings.PressureUnit, _localizationService)));
            Sensors.AddRange(_bench.Sensors.Where(s => s.Sensor is ITenzoSensor)
                                           .Select(s => new TenzoSensorViewModel((ITenzoSensor)s.Sensor, _bench.Settings.TenzoUnit, _localizationService)));
        }

        /// <summary>
        /// Ожидаем остаточное давлен
        /// </summary>
        public void WaitingDefBalloon()
        {
            // Запоминаем старые значения
            //OldTotalDefBalloon = TotalDefBalloon;
            //OldResidualDefBalloon = ResidualDefBalloon;
            TotalDefBalloon = 0;
            ResidualDefBalloon = 0;

            Task.Run(async () =>
            {
                if (_bench.modbusTcpDevice is ModbusTcpDevice modbusTcpDevice)
                {
                    ushort reg = 4116;
                    CancellationToken cancellationToken = new();

                    // Ожидаем от контроллера
                    await _bench.modbusTcpDevice.CreateFloatTriggerAsync(reg, data => data != 0 /*&& data != OldTotalDefBalloon*/,
                        async data =>
                        {
                            TotalDefBalloon = data;
                        },
                        cancellationToken);
                }
            });

            Task.Run(async () =>
            {
                if (_bench.modbusTcpDevice is ModbusTcpDevice modbusTcpDevice)
                {
                    ushort reg1 = 4118;
                    CancellationToken cancellationToken1 = new();

                    // Ожидаем от контроллера
                    await _bench.modbusTcpDevice.CreateFloatTriggerAsync(reg1, data => data != 0 /*&& data != OldResidualDefBalloon*/,
                        async data =>
                        {
                            ResidualDefBalloon = data;
                        },
                        cancellationToken1);
                }
            });
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
        [Reactive]
        public float TotalDefBalloon { get; set; } = 0;
        [Reactive]
        public float ResidualDefBalloon { get; set; } = 0;
        //public float OldTotalDefBalloon { get; set; }
        //public float OldResidualDefBalloon { get; set; }
        [Reactive]
        public ProgrammMethodicsConfig ProgrammMethodicsConfig { get; set; }
        public ObservableCollectionExtended<SensorViewModel> Sensors { get; set; } = new ObservableCollectionExtended<SensorViewModel>();
        public ChartViewModel Chart { get; set; }
        public TemperatureSensorsViewModel TemperatureSensors { get; set; }
        public ReactiveCommand<Unit, Unit> GoParameters { get; set; }
        public ReactiveCommand<Unit, Unit> StartTest { get; set; }
        public ReactiveCommand<Unit, Unit> Exposure { get; set; }
        public ReactiveCommand<Unit, Unit> Result { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
