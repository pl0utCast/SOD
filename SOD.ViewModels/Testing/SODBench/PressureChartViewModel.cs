using ReactiveUI;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Visuals.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using SciChart.Charting.Visuals;
using SciChart.Charting.Common.Extensions;
using System.Windows;
using SciChart.Charting.Common.AttachedProperties;
using System.Windows.Media;
using SciChart.Charting.Visuals.Annotations;
using SOD.Core.Sensor;
using UnitsNet;
using System.Reactive.Linq;
using SOD.LocalizationService;
using SOD.App.Benches.SODBench;
using SciChart.Data.Model;
using UnitsNet.Units;
using System.Linq;
using SciChart.Core.Extensions;
using System.Globalization;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using SOD.Core.Units;
using MemBus;

namespace SOD.ViewModels.Testing.SODBench
{
    public class PressureChartViewModel : ReactiveObject
    {
        private Dictionary<int, XyDataSeries<TimeSpan, double>> pressureSeries = new Dictionary<int, XyDataSeries<TimeSpan, double>>();
        private List<IPressureSensor> pressureSensors = new List<IPressureSensor>();
        private TimeSpan totalTime;
        private IDisposable pressureUpdater;
        private bool isStartChart;
        private readonly ILocalizationService localizationService;
        private readonly Bench bench;
        private NumericAxisViewModel yPressureAxis;
        private NumericAxisViewModel yTenzoAxis;
        private TimeSpanAxisViewModel xTimeSpanAxis;

        public PressureChartViewModel(ILocalizationService localizationService, SOD.App.Benches.SODBench.Bench bench, IBus bus)
        {
            this.localizationService = localizationService;
            this.bench = bench;
            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Green,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yPressureAxis"
            });
            ConfigureAxis();
            bus.Subscribe<App.Benches.SODBench.Messages.SelectedTestMessage>(m =>
            {
                XAxes.Remove(xTimeSpanAxis);
                YAxes.Remove(yPressureAxis);
                YAxes.Remove(yTenzoAxis);
                ConfigureAxis();
            });
        }

        public void ConfigureAxis()
        {
            // конфигурация Axes
            xTimeSpanAxis = new TimeSpanAxisViewModel
            {
                AxisTitle = localizationService["Testing.SODBench.Time"],
                AxisBandsFill = System.Windows.Media.Color.FromArgb(10, 10,10,10),
                DrawMajorBands = true,
                DrawMinorGridLines = false,
                AxisAlignment = AxisAlignment.Bottom,
                StyleKey = "TimeAxisStyle",
                AutoRange = AutoRange.Always,
                HasZoomPanModifier = false
            };
            XAxes.Add(xTimeSpanAxis);

            yPressureAxis = new NumericAxisViewModel
            {
                AxisTitle = localizationService["Testing.SODBench.Pressure"] + ", " + Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name)),
                DrawMajorBands = false,
                DrawMinorGridLines = false,
                MajorDelta = 10,
                MinorDelta = 2,
                AutoTicks = true,
                TextFormatting = "0.0#",
                AxisAlignment = AxisAlignment.Left,
                StyleKey = "PressureAxisStyle",
                HasZoomPanModifier = false,
                BorderThickness= new Thickness(0,0,2,0),
                BorderBrush=System.Windows.Media.Brushes.Green,
                Id = "yPressureAxis"
            };
            YAxes.Add(yPressureAxis);

            yTenzoAxis = new NumericAxisViewModel
            {
                AxisTitle = localizationService["Testing.SODBench.Tenzo"] + ", " + Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name)),
                DrawMajorBands = false,
                DrawMinorGridLines = false,
                MajorDelta = 10,
                MinorDelta = 2,
                AutoTicks = true,
                TextFormatting = "0.0#",
                AxisAlignment = AxisAlignment.Right,
                StyleKey = "TenzoAxisStyle",
                HasZoomPanModifier = false,
                BorderThickness = new Thickness(0, 0, 2, 0),
                BorderBrush = System.Windows.Media.Brushes.Green,
                Id = "yTenzoAxis"
            };
            YAxes.Add(yTenzoAxis);
            
        }
            

        public void SetPressureSensor(IPressureSensor pressureSensor, ITenzoSensor tenzoSensor)
        {
            if (pressureSensor == null) throw new ArgumentNullException(nameof(pressureSensor));

            Pressure step = new Pressure();
            var maxValue = pressureSensor.MaxValue;
            if (maxValue.Bars < 1.0)
                step = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 1.0 && maxValue.Bars <= 1.6)
                step = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 1.6 && maxValue.Bars <= 2.5)
                step = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 2.5 && maxValue.Bars <= 4.0)
                step = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 4.0 && maxValue.Bars <= 6.0)
                step = new Pressure(1, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 6.0 && maxValue.Bars <= 10.0)
                step = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 10.0 && maxValue.Bars <= 16.0)
                step = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 16.0 && maxValue.Bars <= 25.0)
                step = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 25.0 && maxValue.Bars <= 40.0)
                step = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 40.0 && maxValue.Bars <= 60.0)
                step = new Pressure(10, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 60.0 && maxValue.Bars <= 100.0)
                step = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 100.0 && maxValue.Bars <= 160.0)
                step = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 160.0 && maxValue.Bars <= 250.0)
                step = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 250.0 && maxValue.Bars <= 400.0)
                step = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 400.0 && maxValue.Bars <= 600.0)
                step = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
            else if (maxValue.Bars > 600.0)
                step = new Pressure(200, UnitsNet.Units.PressureUnit.Bar);

            yPressureAxis.VisibleRange = new DoubleRange(pressureSensor.MinValue.ToUnit(bench.Settings.PressureUnit).Value, pressureSensor.MaxValue.ToUnit(bench.Settings.PressureUnit).Value * 1.1);
            yPressureAxis.MajorDelta = step.ToUnit(bench.Settings.PressureUnit).Value;
            yPressureAxis.MinorDelta = step.ToUnit(bench.Settings.PressureUnit).Value/10.0;

            if (!pressureSensors.Contains(pressureSensor))
            {
                pressureSensors.Add(pressureSensor);
            }
            if (!pressureSeries.ContainsKey(pressureSensor.Id))
            {
                var pressSeries = new XyDataSeries<TimeSpan, double>();
                //var tenzoSeries = new XyDataSeries<TimeSpan, double>();
                PressureSeries.Add(new LineRenderableSeriesViewModel() { DataSeries = pressSeries, AntiAliasing = true, Stroke = Colors.Red, StrokeThickness = 2, YAxisId = "yPressureAxis" });
                pressureSeries.Add(pressureSensor.Id, pressSeries);
                //PressureSeries.Add(new LineRenderableSeriesViewModel() { DataSeries = tenzoSeries, AntiAliasing = true, Stroke = Colors.Brown, StrokeThickness = 2, YAxisId = "yTenzoAxis" });
                //pressureSeries.Add(tenzoSensor.Id, tenzoSeries);
            }
        }

        public void StartChart()
        {
            if (isStartChart) return;

            foreach (var series in pressureSeries.Values)
            {
                series.Clear();
            }

            pressureUpdater = Observable.Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
                                        .Subscribe(time =>
                                        {
                                            totalTime = TimeSpan.FromMilliseconds(time*100);
                                            foreach (var pressureSensor in pressureSensors.ToList())
                                            {
                                                double currentPressure = Math.Round(pressureSensor.Pressure.ToUnit(bench.Settings.PressureUnit).Value, 2);
                                                pressureSeries[pressureSensor.Id].Append(totalTime, currentPressure);

                                                if (bench.Settings.AutoRange)
                                                    yPressureAxis.VisibleRange = new DoubleRange(PressureSeries[0].DataSeries.YMin.ToDouble() - Math.Abs(PressureSeries[0].DataSeries.YMin.ToDouble() * 5 / 100), PressureSeries[0].DataSeries.YMax.ToDouble() + Math.Abs(PressureSeries[0].DataSeries.YMax.ToDouble() * 5 / 100));
                                            }
                                        });
            isStartChart = true;
        }

        public void StopChart()
        {
            if (!isStartChart) return;

            pressureUpdater?.Dispose();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            isStartChart = false;
        }

        public void ClearChart()
        {
            Annotations.Clear();
            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Green,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yPressureAxis"
            });
            pressureSeries.Clear();
            pressureSensors.Clear();
            PressureSeries.Clear();
        }

        public void SetAnnotation()
        {
            Annotations.Add(new VerticalLineAnnotationViewModel
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                ShowLabel = false,
                Stroke = Colors.Blue,
                LabelValue = "Stop",
                LabelPlacement = LabelPlacement.Bottom,
                StrokeThickness = 2,
                X1 = totalTime,
                IsEditable = false,
            });      
        }

        public ObservableCollection<IAnnotationViewModel> Annotations { get; set; } = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IRenderableSeriesViewModel> PressureSeries { get; set; } = new ObservableCollection<IRenderableSeriesViewModel>();
        public ObservableCollection<IAxisViewModel> XAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollection<IAxisViewModel> YAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
    }
}
