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
using SciChart.Charting.Visuals.Axes.LabelProviders;

namespace SOD.ViewModels.Testing.SODBench
{
    public class ChartViewModel : ReactiveObject
    {
        private Dictionary<int, XyDataSeries<TimeSpan, double>> pressureSeries = new Dictionary<int, XyDataSeries<TimeSpan, double>>();
        private Dictionary<int, XyDataSeries<TimeSpan, double>> tenzoSeries = new Dictionary<int, XyDataSeries<TimeSpan, double>>();
        private List<IPressureSensor> pressureSensors = new List<IPressureSensor>();
        private List<ITenzoSensor> tenzoSensors = new List<ITenzoSensor>();
        private TimeSpan totalTime;
        private IDisposable Updater;
        private bool isStartChart;
        private readonly ILocalizationService localizationService;
        private readonly Bench bench;
        private NumericAxisViewModel yPressureAxis;
        private NumericAxisViewModel yTenzoAxis;
        private TimeSpanAxisViewModel xTimeSpanAxis;

        public ChartViewModel(ILocalizationService localizationService, SOD.App.Benches.SODBench.Bench bench, IBus bus)
        {
            this.localizationService = localizationService;
            this.bench = bench;
            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Red,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yPressureAxis"
            });
            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Blue,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yTenzoAxis"
            });

            ConfigureAxis();
            bus.Subscribe<App.Benches.SODBench.Messages.SelectedTestMessage>(m =>
            {
                Series[0].DataSeries.SeriesName = localizationService["Testing.SODBench.Pressure"] + ", " + Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name));
                Series[1].DataSeries.SeriesName = localizationService["Testing.SODBench.Tenzo"] + ", " + Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name));
                yPressureAxis.AxisTitle = localizationService["Testing.SODBench.Pressure"] + ", " + Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name));
                yTenzoAxis.AxisTitle = localizationService["Testing.SODBench.Tenzo"] + ", " + Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name));
            });
        }

        public void ConfigureAxis()
        {
            // конфигурация Axes
            xTimeSpanAxis = new TimeSpanAxisViewModel
            {
                AxisTitle = localizationService["Testing.SODBench.Time"],
                AxisBandsFill = System.Windows.Media.Color.FromArgb(10, 10, 10, 10),
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
                BorderThickness = new Thickness(0, 0, 2, 0),
                BorderBrush = System.Windows.Media.Brushes.Red,
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
                BorderBrush = System.Windows.Media.Brushes.Blue,
                Id = "yTenzoAxis"
            };
            YAxes.Add(yTenzoAxis);

        }


        public void SetPressureSensor(IPressureSensor pressureSensor, ITenzoSensor tenzoSensor)
        {
            if (pressureSensor == null) throw new ArgumentNullException(nameof(pressureSensor));
            if (tenzoSensor == null) throw new ArgumentNullException(nameof(tenzoSensor));

            Pressure pStep = new Pressure();
            var pMaxValue = pressureSensor.MaxValue;
            if (pMaxValue.Bars < 1.0)
                pStep = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 1.0 && pMaxValue.Bars <= 1.6)
                pStep = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 1.6 && pMaxValue.Bars <= 2.5)
                pStep = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 2.5 && pMaxValue.Bars <= 4.0)
                pStep = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 4.0 && pMaxValue.Bars <= 6.0)
                pStep = new Pressure(1, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 6.0 && pMaxValue.Bars <= 10.0)
                pStep = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 10.0 && pMaxValue.Bars <= 16.0)
                pStep = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 16.0 && pMaxValue.Bars <= 25.0)
                pStep = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 25.0 && pMaxValue.Bars <= 40.0)
                pStep = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 40.0 && pMaxValue.Bars <= 60.0)
                pStep = new Pressure(10, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 60.0 && pMaxValue.Bars <= 100.0)
                pStep = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 100.0 && pMaxValue.Bars <= 160.0)
                pStep = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 160.0 && pMaxValue.Bars <= 250.0)
                pStep = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 250.0 && pMaxValue.Bars <= 400.0)
                pStep = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 400.0 && pMaxValue.Bars <= 600.0)
                pStep = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
            else if (pMaxValue.Bars > 600.0)
                pStep = new Pressure(200, UnitsNet.Units.PressureUnit.Bar);

            Force tStep = new Force();
            var tMaxValue = tenzoSensor.MaxValue;
            if (tMaxValue.KilogramsForce < 1.0)
                tStep = new Force(0.2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 1.0 && tMaxValue.KilogramsForce <= 1.6)
                tStep = new Force(0.2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 1.6 && tMaxValue.KilogramsForce <= 2.5)
                tStep = new Force(0.5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 2.5 && tMaxValue.KilogramsForce <= 4.0)
                tStep = new Force(0.5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 4.0 && tMaxValue.KilogramsForce <= 6.0)
                tStep = new Force(1, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 6.0 && tMaxValue.KilogramsForce <= 10.0)
                tStep = new Force(2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 10.0 && tMaxValue.KilogramsForce <= 16.0)
                tStep = new Force(2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 16.0 && tMaxValue.KilogramsForce <= 25.0)
                tStep = new Force(5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 25.0 && tMaxValue.KilogramsForce <= 40.0)
                tStep = new Force(5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 40.0 && tMaxValue.KilogramsForce <= 60.0)
                tStep = new Force(10, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 60.0 && tMaxValue.KilogramsForce <= 100.0)
                tStep = new Force(20, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 100.0 && tMaxValue.KilogramsForce <= 160.0)
                tStep = new Force(20, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 160.0 && tMaxValue.KilogramsForce <= 250.0)
                tStep = new Force(50, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 250.0 && tMaxValue.KilogramsForce <= 400.0)
                tStep = new Force(50, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 400.0 && tMaxValue.KilogramsForce <= 600.0)
                tStep = new Force(100, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tMaxValue.KilogramsForce > 600.0)
                tStep = new Force(200, UnitsNet.Units.ForceUnit.KilogramForce);

            yPressureAxis.VisibleRange = new DoubleRange(pressureSensor.MinValue.ToUnit(bench.Settings.PressureUnit).Value, pressureSensor.MaxValue.ToUnit(bench.Settings.PressureUnit).Value * 1.1);
            yPressureAxis.MajorDelta = pStep.ToUnit(bench.Settings.PressureUnit).Value;
            yPressureAxis.MinorDelta = pStep.ToUnit(bench.Settings.PressureUnit).Value / 10.0;

            yTenzoAxis.VisibleRange = new DoubleRange(tenzoSensor.MinValue.ToUnit(bench.Settings.TenzoUnit).Value, tenzoSensor.MaxValue.ToUnit(bench.Settings.TenzoUnit).Value * 1.1);
            yTenzoAxis.MajorDelta = tStep.ToUnit(bench.Settings.TenzoUnit).Value;
            yTenzoAxis.MinorDelta = tStep.ToUnit(bench.Settings.TenzoUnit).Value / 10.0;

            if (!pressureSensors.Contains(pressureSensor))
            {
                pressureSensors.Add(pressureSensor);
            }
            if (!pressureSeries.ContainsKey(pressureSensor.Id))
            {
                var pressSeries = new XyDataSeries<TimeSpan, double>()
                {
                    SeriesName = localizationService["Testing.SODBench.Pressure"] + ", " + Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name)),
                };

                Series.Add(new LineRenderableSeriesViewModel()
                {
                    DataSeries = pressSeries,
                    AntiAliasing = true,
                    Stroke = Colors.Red,
                    StrokeThickness = 2,
                    YAxisId = "yPressureAxis",
                });
                pressureSeries.Add(pressureSensor.Id, pressSeries);
            }

            if (!tenzoSensors.Contains(tenzoSensor)) 
                tenzoSensors.Add(tenzoSensor);

            if (!tenzoSeries.ContainsKey(tenzoSensor.Id))
            {
                var tenzSeries = new XyDataSeries<TimeSpan, double>()
                {
                    SeriesName = localizationService["Testing.SODBench.Tenzo"] + ", " + Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name)),
                };

                Series.Add(new LineRenderableSeriesViewModel()
                {
                    DataSeries = tenzSeries,
                    AntiAliasing = true,
                    Stroke = Colors.Blue,
                    StrokeThickness = 2,
                    YAxisId = "yTenzoAxis"
                });
                tenzoSeries.Add(tenzoSensor.Id, tenzSeries);
            }
        }

        private void ClearSeries()
        {
            pressureSeries.Values.FirstOrDefault().Clear();

            tenzoSeries.Values.FirstOrDefault().Clear();
        }

        public void StartChart()
        {
            if (isStartChart)
            {
                return;
            }

            ClearSeries();

            Updater = Observable.Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
                                        .Subscribe(time =>
                                        {
                                            totalTime = TimeSpan.FromMilliseconds(time * 100);

                                            var pressureSensor = pressureSensors.FirstOrDefault();
                                            double currentPressure = Math.Round(pressureSensor.Pressure.ToUnit(bench.Settings.PressureUnit).Value, 2);
                                            pressureSeries[pressureSensor.Id].Append(totalTime, currentPressure);

                                            if (bench.Settings.AutoRange)
                                                yPressureAxis.VisibleRange = new DoubleRange(Series[0].DataSeries.YMin.ToDouble() - Math.Abs(Series[0].DataSeries.YMin.ToDouble() * 5 / 100), Series[0].DataSeries.YMax.ToDouble() + Math.Abs(Series[0].DataSeries.YMax.ToDouble() * 5 / 100));

                                            var tenzoSensor = tenzoSensors.FirstOrDefault();
                                            double currentTenzo = Math.Round(tenzoSensor.Force.ToUnit(bench.Settings.TenzoUnit).Value, 2);
                                            tenzoSeries[tenzoSensor.Id].Append(totalTime, currentTenzo);

                                            if (bench.Settings.AutoRange)
                                                yTenzoAxis.VisibleRange = new DoubleRange(Series[1].DataSeries.YMin.ToDouble() - Math.Abs(Series[1].DataSeries.YMin.ToDouble() * 5 / 100), Series[1].DataSeries.YMax.ToDouble() + Math.Abs(Series[1].DataSeries.YMax.ToDouble() * 5 / 100));
                                        });
            isStartChart = true;
        }

        public void StopChart()
        {
            if (!isStartChart)
            {
                return;
            }

            Updater?.Dispose();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            isStartChart = false;
        }

        public void ClearChart()
        {
            Annotations.Clear();
            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Red,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yPressureAxis"
            });

            Annotations.Add(new HorizontalLineAnnotationViewModel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = Colors.Blue,
                StrokeThickness = 1,
                Y1 = 0,
                X1 = 0,
                IsEditable = false,
                YAxisId = "yTenzoAxis"
            });

            pressureSeries.Clear();
            pressureSensors.Clear();

            tenzoSeries.Clear();
            tenzoSensors.Clear();

            Series.Clear();
        }

        public void SetAnnotation()
        {
            Annotations.Add(new VerticalLineAnnotationViewModel
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                ShowLabel = false,
                Stroke = Colors.Green,
                LabelValue = "Stop",
                LabelPlacement = LabelPlacement.Bottom,
                StrokeThickness = 2,
                X1 = totalTime,
                IsEditable = false,
            });
        }

        public ObservableCollection<IAnnotationViewModel> Annotations { get; set; } = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IRenderableSeriesViewModel> Series { get; set; } = new ObservableCollection<IRenderableSeriesViewModel>();
        public ObservableCollection<IAxisViewModel> XAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollection<IAxisViewModel> YAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
    }
}
