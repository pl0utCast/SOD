using DynamicData.Binding;
using MemBus;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Core.Valves;
using SOD.App.Benches.CRSBench;
using SOD.App.Benches.CRSBench.Messages;
using SOD.App.Testing.Funcional;
using SOD.ViewModels.Extensions;
using SOD.ViewModels.Testing.CRSBench.Sensors;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SciChart.Charting.Model.ChartData;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.Axes;
using SciChart.Core.Extensions;
using SciChart.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class FuncionalityTestViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly ISensorService sensorService;
        private readonly ILocalizationService localizationService;
        private readonly Bench _bench;
        private NumericAxisViewModel yPressureAxis;
        private Stopwatch stopwatch = new Stopwatch();
        private IDisposable pressureUpdaterDisposable;
        private TimeSpanAxisViewModel xTimeSpanAxis;
        private BoxAnnotationViewModel expectedSetPressureAnotation;
        private VerticalLineAnnotationViewModel openPressureAnnotation;
        private VerticalLineAnnotationViewModel closePressureAnnotation;
        private bool isAddTestToReport = false;
        public FuncionalityTestViewModel(INavigationService navigationService,
                                         ISensorService sensorService,
                                         IBus bus,
                                         ILocalizationService localizationService,
                                         IDialogService dialogService,
                                         Bench bench)
        {
            _bench = bench;
            this.sensorService = sensorService;
            this.localizationService = localizationService;
            TemperatureSensors = new TemperatureSensorsViewModel(sensorService, bench);
            bus.Subscribe<SelectedTestMessage>(t =>
            {
                if (t.TestType == App.Testing.TestType.Functional)
                {
                    UpdateSensors();
                    IsSelectedTest = true;
                    IsTestResultFill = false;
                }
            });

            var canGoParameters = this.WhenAnyValue(x => x.IsRunTest, x => x.IsSelectOpenPoint, x => x.IsSelectClosePoint,
                (isRunTest, isSelectOpen, isSelectClose) => !isRunTest && !isSelectOpen && !isSelectClose);
            GoParameters = ReactiveCommand.Create(() =>
            {
                navigationService.NavigateTo("CRSTestParameters");
            }, canGoParameters);


            var canStartTest = this.WhenAnyValue(x => x.IsSelectedTest, x => x.IsSelectOpenPoint, x => x.IsSelectClosePoint,
                (isSelectTest, isSelectOpenPoint, isSelectClosePoint) => isSelectTest && !isSelectOpenPoint && !isSelectClosePoint);
            StartTest = ReactiveCommand.Create(() =>
            {
                if (IsRunTest)
                {
                    StopChart();
                    xTimeSpanAxis.AutoRange = AutoRange.Never;
                    //yPressureAxis.AutoRange = AutoRange.Never;
                    IsRunTest = false;
                    _bench.StopTesting();
                    var test = _bench.CurrentTest as Test;
                    if (test.TestResult.PostResults.Count()>0)
                    {
                        SetOpenPressure(test.TestResult.PostResults.First().OpenPoint);
                        SetClosePressure(test.TestResult.PostResults.First().ClosePoint);
                        //yPressureAxis.VisibleRange.SetMinMax((double)yPressureAxis.VisibleRange.Min, (double)yPressureAxis.VisibleRange.Max * 1.1);
                        IsTestResultFill = true;
                        isAddTestToReport = true;
                    }
                    bus.Publish(new App.Messages.ProgrammMethodicsStatus(App.Messages.ProgrammStatus.Stop));

                }
                else
                {
                    
                    var expectedSetPressure = _bench.Settings.SelectedTestSettings.SetPressure.ToUnit(_bench.Settings.PressureUnit).Value;
                    if (bench.Settings.SelectedTestSettings.UseValveSetPressure)
                    {
                        expectedSetPressure = ((Pressure)bench.TestingValve?.GetValveProperty("set_pressure")?.Value).ToUnit(_bench.Settings.PressureUnit).Value;
                    }
                    expectedSetPressureAnotation.Y1 = (expectedSetPressure * _bench.Settings.FuncionalityTestValidSetPressurePercent / 100) + expectedSetPressure;
                    expectedSetPressureAnotation.Y2 = expectedSetPressure - (expectedSetPressure * _bench.Settings.FuncionalityTestValidSetPressurePercent / 100);
                    expectedSetPressureAnotation.X1 = TimeSpan.FromSeconds(0);
                    expectedSetPressureAnotation.X2 = TimeSpan.FromSeconds(1000);
                    xTimeSpanAxis.AutoRange = AutoRange.Always;
                    //yPressureAxis.AutoRange = AutoRange.Always;
                    openPressureAnnotation.IsHidden = true;
                    closePressureAnnotation.IsHidden = true;
                    StartChart();
                    IsRunTest = true;
                    _bench.StartTesting();
                    bus.Publish(new App.Messages.ProgrammMethodicsStatus(App.Messages.ProgrammStatus.Run));
                }    
            }, canStartTest);

            var canResult = this.WhenAnyValue(x => x.IsRunTest, x => x.IsTestResultFill, x=>x.IsSelectOpenPoint, x=>x.IsSelectClosePoint,
                (isRunTest, isTestResultFill, isSelectOpenPoint, isSelectClosePoint) => !isRunTest && isTestResultFill && !isSelectOpenPoint && !isSelectClosePoint);

            Result = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new Dialogs.FuncionalTestResultViewModel(((Test)_bench.CurrentTest).TestResult.PostResults.FirstOrDefault(),
                                                                localizationService,
                                                                dialogService,
                                                                Observable.Return(isAddTestToReport));
                var result = await dialogService.ShowDialogAsync("CRSBenchTestResult", vm);
                if (result != null)
                {
                    isAddTestToReport = false;
                    _bench.UpdateReport(PressureSerie.ParentSurface.ExportToBitmapSource().GetBitmap());
                }
            }, canResult);

            // конфигурация Axes
            xTimeSpanAxis = new TimeSpanAxisViewModel
            {
                AxisTitle = localizationService["Testing.CRSBench.Time"],
                AxisBandsFill = System.Windows.Media.Color.FromArgb(10, 10, 10, 10),
                DrawMajorBands = true,
                DrawMinorGridLines = false,
                AxisAlignment = AxisAlignment.Bottom,
                StyleKey = "TimeAxisStyle",
                AutoRange = AutoRange.Always,
                HasZoomPanModifier = true
            };
            XAxes.Add(xTimeSpanAxis);
            yPressureAxis = new NumericAxisViewModel
            {

                AxisTitle = localizationService["Testing.CRSBench.Pressure"],
                DrawMajorBands = false,
                DrawMinorGridLines = false,
                MajorDelta = 10,
                MinorDelta = 2,
                AutoTicks = true,
                TextFormatting = "0.00#",
                AxisAlignment = AxisAlignment.Left,
                StyleKey = "PressureAxisStyle",
                HasZoomPanModifier = true,
                BorderThickness = new Thickness(0, 0, 2, 0),
                BorderBrush = System.Windows.Media.Brushes.Green,
                AutoRange = AutoRange.Never,
                VisibleRange = PressureRange
            };
            YAxes.Add(yPressureAxis);

            expectedSetPressureAnotation = new BoxAnnotationViewModel();
            expectedSetPressureAnotation.Background = new SolidColorBrush(Color.FromArgb(125, 0xff, 0xf2, 00));
            //expectedSetPressureAnotation.StrokeThickness = 20;
            Annotations.Add(expectedSetPressureAnotation);

            openPressureAnnotation = new VerticalLineAnnotationViewModel();
            openPressureAnnotation.IsEditable = false;
            openPressureAnnotation.Stroke = Color.FromArgb(0xff, 0, 0, 0);
            openPressureAnnotation.StrokeDashArray = new DoubleCollection(new [] {3.0,3.0});
            openPressureAnnotation.IsHidden=true;
            openPressureAnnotation.LabelPlacement = SciChart.Charting.Visuals.Annotations.LabelPlacement.Axis;
            openPressureAnnotation.ShowLabel = true;
            openPressureAnnotation.LabelValue = localizationService["Testing.CRSBench.OpenPressure"];
            Annotations.Add(openPressureAnnotation);

            closePressureAnnotation = new VerticalLineAnnotationViewModel();
            closePressureAnnotation.IsEditable = false;
            closePressureAnnotation.Stroke = Color.FromArgb(0xff, 0, 0, 0);
            closePressureAnnotation.StrokeDashArray = new DoubleCollection(new[] { 3.0, 3.0 });
            closePressureAnnotation.IsHidden = true;
            closePressureAnnotation.LabelPlacement = SciChart.Charting.Visuals.Annotations.LabelPlacement.Axis;
            closePressureAnnotation.ShowLabel = true;
            closePressureAnnotation.LabelValue = localizationService["Testing.CRSBench.ClosePressure"];
            Annotations.Add(closePressureAnnotation);

            PressureSeries.Add(new LineRenderableSeriesViewModel() { DataSeries = PressureSerie, AntiAliasing = true, Stroke = Colors.Red, StrokeThickness = 2 });

            this.WhenActivated(dis =>
            {
                bus.Subscribe<App.Messages.Reports.ReportSaveMessage>(m =>
                {
                    IsTestResultFill = false;
                }).DisposeWith(dis);
                bus.Subscribe<App.Messages.Reports.CreateNewReportMessage>(m =>
                {
                    IsTestResultFill = false;
                }).DisposeWith(dis);
                this.WhenAnyValue(x => x.IsSelectOpenPoint)
                    .DistinctUntilChanged()
                    .Where(isSelect=>isSelect)
                    .Subscribe((isSelect) =>
                    {
                        IsSelectClosePoint = false;
                    })
                    .DisposeWith(dis);
                this.WhenAnyValue(x => x.IsSelectClosePoint)
                    .DistinctUntilChanged()
                    .Where(isSelect => isSelect)
                    .Subscribe((isSelect) =>
                    {
                        IsSelectOpenPoint = false;
                    })
                    .DisposeWith(dis);
                this.WhenAnyValue(x => x.SelectedSeriesInfo)
                    .Where(si => si != null)
                    .Subscribe(seriesInfo =>
                    {
                        if (IsSelectOpenPoint)
                        {
                            SetOpenPressure(seriesInfo.DataSeriesIndex);
                        }
                        else if (IsSelectClosePoint)
                        {
                            SetClosePressure(seriesInfo.DataSeriesIndex);
                        }
                    })
                    .DisposeWith(dis);

            });
            UpdateSensors();

        }

        public void StartChart()
        {
            //PressureSeries.Clear();
            //var pressSeries = new XyDataSeries<TimeSpan, double>();
            //PressureSeries.Add(new LineRenderableSeriesViewModel() { DataSeries = pressSeries, AntiAliasing = true, Stroke = Colors.Red, StrokeThickness = 2 });
            PressureSerie.Clear();

            var pressureSensor = sensorService.GetSensor(_bench.Settings.SelectedTestSettings.PressureSensorId.Value) as IPressureSensor;
            stopwatch.Restart();

            pressureUpdaterDisposable = pressureSensor.Subscribe(pressure =>
            {
                var press = pressure.ToUnit(_bench.Settings.PressureUnit).Value;

                PressureRange.Min = PressureSerie.YMin.ToDouble() - Math.Abs(PressureSerie.YMin.ToDouble() * 5 / 100);
                PressureRange.Max = PressureSerie.YMax.ToDouble() + Math.Abs(PressureSerie.YMax.ToDouble() * 5 / 100);

                PressureSerie.Append(stopwatch.Elapsed, press);
            });
        }

        public void StopChart()
        {
            pressureUpdaterDisposable?.Dispose();
        }

        private void UpdateSensors()
        {
            yPressureAxis.AxisTitle = localizationService["Testing.CRSBench.Pressure"] +
                " (" + UnitsNetSetup.Default.UnitAbbreviations.GetDefaultAbbreviation(typeof(PressureUnit), (int)_bench.Settings.PressureUnit) + ")";
            Sensors.Clear();
            foreach (var post in _bench.Posts)
            {
                Sensors.AddRange(post.Sensors
                                     .Where(s => s.Sensor is IPressureSensor)
                                     .Select(s => new PressureSensorViewModel((IPressureSensor)s.Sensor, _bench.Settings.PressureUnit)));
            }
        }

        private void SetOpenPressure(int index)
        {
            var pressure = index == 0 ? new Pressure().ToUnit(_bench.Settings.PressureUnit).Value : PressureSerie.YValues[index];
            var newPressure = new Pressure(pressure, _bench.Settings.PressureUnit);
            OpenPressure = newPressure.ToString("f2");
            var test = _bench.CurrentTest as Test;
            openPressureAnnotation.X1 = PressureSerie.XValues[index];
            openPressureAnnotation.IsHidden = false;
            test.TestResult.PostResults.FirstOrDefault().OpenPressure = newPressure;
            test.TestResult.PostResults.FirstOrDefault().OpenPoint = index;
            OpenPressurePercent = test.TestResult.PostResults.First().Accuracy;
        }

        private void SetClosePressure(int index)
        {
            var pressure = index == 0 ? new Pressure().ToUnit(_bench.Settings.PressureUnit).Value : PressureSerie.YValues[index];
            var newPressure = new Pressure(pressure, _bench.Settings.PressureUnit);
            ClosePressure = newPressure.ToString("f2");
            closePressureAnnotation.X1 = PressureSerie.XValues[index];
            closePressureAnnotation.IsHidden = false;

            var test = _bench.CurrentTest as Test;
            test.TestResult.PostResults.FirstOrDefault().ClosePressure = newPressure;
            test.TestResult.PostResults.FirstOrDefault().ClosePoint = index;
        }

        [Reactive]
        public double OpenPressurePercent { get; set; }
        [Reactive]
        public string OpenPressure { get; set; }
        [Reactive]
        public string ClosePressure { get; set; }
        [Reactive]
        public bool IsRunTest { get; set; }
        [Reactive]
        public bool IsSelectedTest { get; set; }
        [Reactive]
        public bool IsTestResultFill { get; set; }
        [Reactive]
        public bool IsSelectOpenPoint { get; set; }
        [Reactive]
        public bool IsSelectClosePoint { get; set; }
        [Reactive]
        public SeriesInfo SelectedSeriesInfo { get; set; }
        public DoubleRange PressureRange { get; set; } = new DoubleRange(0, 1);
        public ObservableCollection<IAnnotationViewModel> Annotations { get; set; } = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IRenderableSeriesViewModel> PressureSeries { get; set; } = new ObservableCollection<IRenderableSeriesViewModel>();
        public XyDataSeries<TimeSpan, double> PressureSerie { get; set; } = new XyDataSeries<TimeSpan, double>();
        public ObservableCollection<IAxisViewModel> XAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollection<IAxisViewModel> YAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollectionExtended<SensorViewModel> Sensors { get; set; } = new ObservableCollectionExtended<SensorViewModel>();
        public PressureChartViewModel PressureChart { get; set; }
        public TemperatureSensorsViewModel TemperatureSensors { get; set; }
        public ReactiveCommand<Unit, Unit> GoParameters { get; set; }
        public ReactiveCommand<Unit, Unit> StartTest { get; set; }
        public ReactiveCommand<Unit, Unit> Result { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        
    }
}
