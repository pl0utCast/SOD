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
using SciChart.Charting.ChartModifiers;
using SOD.Core.Sensor.TenzoSensor.CodeBased;
using SOD.Core.Sensor.PressureSensor;
using MemBus.Support;

namespace SOD.ViewModels.Testing.SODBench
{
    public class ChartViewModel : ReactiveObject
    {
        private Dictionary<int, XyDataSeries<TimeSpan, double>> pressureTimeSeriesDict = new Dictionary<int, XyDataSeries<TimeSpan, double>>();
        private Dictionary<int, XyDataSeries<TimeSpan, double>> tenzoTimeSeriesDict = new Dictionary<int, XyDataSeries<TimeSpan, double>>();
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
        private string pressureUnitAbbreviation;
        private string forceUnitAbbreviation;

        public ChartViewModel(ILocalizationService localizationService, SOD.App.Benches.SODBench.Bench bench, IBus bus)
        {
            this.localizationService = localizationService;
            this.bench = bench;
            pressureUnitAbbreviation = Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name));
            forceUnitAbbreviation = Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name));
            ConfigureAxes();
            SetAnnotations();
            bus.Subscribe<App.Benches.SODBench.Messages.SelectedTestMessage>(m =>
            {
                ClearChartLines();
                pressureUnitAbbreviation = Pressure.GetAbbreviation(bench.Settings.PressureUnit, new CultureInfo(localizationService.CurrentCulture.Name));
                forceUnitAbbreviation = Force.GetAbbreviation(bench.Settings.TenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name));
                Series[0].DataSeries.SeriesName = localizationService["Testing.SODBench.Pressure"] + ", " + pressureUnitAbbreviation;
                Series[1].DataSeries.SeriesName = localizationService["Testing.SODBench.Tenzo"] + ", " + forceUnitAbbreviation;
                yPressureAxis.AxisTitle = localizationService["Testing.SODBench.Pressure"] + ", " + pressureUnitAbbreviation;
                yTenzoAxis.AxisTitle = localizationService["Testing.SODBench.Tenzo"] + ", " + forceUnitAbbreviation;
            });
        }

        public void ConfigureAxes()
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
                AxisTitle = localizationService["Testing.SODBench.Pressure"] + ", " + pressureUnitAbbreviation,
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
                AxisTitle = localizationService["Testing.SODBench.Tenzo"] + ", " + forceUnitAbbreviation,
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

        public void SetSensors(IPressureSensor pressureSensor, ITenzoSensor tenzoSensor)
        {
            if (pressureSensor == null) throw new ArgumentNullException(nameof(pressureSensor));
            if (tenzoSensor == null) throw new ArgumentNullException(nameof(tenzoSensor));

            ConfigureAxesScaling(pressureSensor, tenzoSensor);

            Series.Clear();
            pressureSensors.Clear();
            tenzoSensors.Clear();
            pressureTimeSeriesDict.Clear();
            tenzoTimeSeriesDict.Clear();

            pressureSensors.Add(pressureSensor);
            tenzoSensors.Add(tenzoSensor);

            var pressureSeries = new XyDataSeries<TimeSpan, double>()
            {
                SeriesName = localizationService["Testing.SODBench.Pressure"] + ", " + pressureUnitAbbreviation,
            };

            var tenzoSeries = new XyDataSeries<TimeSpan, double>()
            {
                SeriesName = localizationService["Testing.SODBench.Tenzo"] + ", " + forceUnitAbbreviation,
            };

            Series.Add(new LineRenderableSeriesViewModel()
            {
                DataSeries = pressureSeries,
                AntiAliasing = true,
                Stroke = Colors.Red,
                StrokeThickness = 2,
                YAxisId = "yPressureAxis",
            });

            Series.Add(new LineRenderableSeriesViewModel()
            {
                DataSeries = tenzoSeries,
                AntiAliasing = true,
                Stroke = Colors.Blue,
                StrokeThickness = 2,
                YAxisId = "yTenzoAxis",
            });

            pressureTimeSeriesDict.Add(pressureSensor.Id, pressureSeries);
            tenzoTimeSeriesDict.Add(tenzoSensor.Id, tenzoSeries);
        }


        public void StartChart()
        {
            if (isStartChart)
            {
                return;
            }

            ClearChartLines();

            Updater = Observable.Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
                                .Subscribe(time =>
                                {
                                    totalTime = TimeSpan.FromMilliseconds(time * 100);

                                    var pressureSensor = pressureSensors.FirstOrDefault();
                                    double currentPressure = Math.Round(pressureSensor.Pressure.ToUnit(bench.Settings.PressureUnit).Value, 2);
                                    pressureTimeSeriesDict[pressureSensor.Id].Append(totalTime, currentPressure);

                                    SetAutoRangeIfEnabled(Series[0].DataSeries, yPressureAxis);

                                    var tenzoSensor = tenzoSensors.FirstOrDefault();
                                    double currentTenzo = Math.Round(tenzoSensor.Force.ToUnit(bench.Settings.TenzoUnit).Value, 2);
                                    tenzoTimeSeriesDict[tenzoSensor.Id].Append(totalTime, currentTenzo);

                                    SetAutoRangeIfEnabled(Series[1].DataSeries, yTenzoAxis);
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

            Series.Clear();
        }

        public void SetAnnotations()
        {
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
        }

        public void SetExposureLine()
        {
            Annotations.Add(new VerticalLineAnnotationViewModel
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                YAxisId = "tenzoSeries",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                ShowLabel = false,
                Stroke = Colors.Blue,
                LabelValue = "Stop",
                LabelPlacement = LabelPlacement.Bottom,
                StrokeThickness = 2,
                X1 = totalTime,
                IsEditable = false
            });
        }
        private void ClearChartLines()
        {
            foreach (var series in pressureTimeSeriesDict.Values)
                series.Clear();

            foreach (var series in tenzoTimeSeriesDict.Values)
                series.Clear();
        }

        private void SetAutoRangeIfEnabled(IDataSeries dataSeries, NumericAxisViewModel yAxis)
        {
            if (bench.Settings.AutoRange)
            {
                var yAxisMinValue = dataSeries.YMin.ToDouble() - Math.Abs(dataSeries.YMin.ToDouble() * 5 / 100);
                var yAxisMaxValue = dataSeries.YMax.ToDouble() + Math.Abs(dataSeries.YMax.ToDouble() * 5 / 100);
                yAxis.VisibleRange = new DoubleRange(yAxisMinValue, yAxisMaxValue);
            }
        }

        private void ConfigureAxesScaling(IPressureSensor pressureSensor, ITenzoSensor tenzoSensor)
        {
            Pressure pressureStep = new Pressure();
            var pressureMaxValue = pressureSensor.MaxValue;
            if (pressureMaxValue.Bars < 1.0)
                pressureStep = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 1.0 && pressureMaxValue.Bars <= 1.6)
                pressureStep = new Pressure(0.2, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 1.6 && pressureMaxValue.Bars <= 2.5)
                pressureStep = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 2.5 && pressureMaxValue.Bars <= 4.0)
                pressureStep = new Pressure(0.5, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 4.0 && pressureMaxValue.Bars <= 6.0)
                pressureStep = new Pressure(1, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 6.0 && pressureMaxValue.Bars <= 10.0)
                pressureStep = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 10.0 && pressureMaxValue.Bars <= 16.0)
                pressureStep = new Pressure(2, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 16.0 && pressureMaxValue.Bars <= 25.0)
                pressureStep = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 25.0 && pressureMaxValue.Bars <= 40.0)
                pressureStep = new Pressure(5, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 40.0 && pressureMaxValue.Bars <= 60.0)
                pressureStep = new Pressure(10, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 60.0 && pressureMaxValue.Bars <= 100.0)
                pressureStep = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 100.0 && pressureMaxValue.Bars <= 160.0)
                pressureStep = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 160.0 && pressureMaxValue.Bars <= 250.0)
                pressureStep = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 250.0 && pressureMaxValue.Bars <= 400.0)
                pressureStep = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 400.0 && pressureMaxValue.Bars <= 600.0)
                pressureStep = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
            else if (pressureMaxValue.Bars > 600.0)
                pressureStep = new Pressure(200, UnitsNet.Units.PressureUnit.Bar);

            Force tenzoStep = new Force();
            var tenzoMaxValue = tenzoSensor.MaxValue;
            if (tenzoMaxValue.KilogramsForce < 1.0)
                tenzoStep = new Force(0.2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 1.0 && tenzoMaxValue.KilogramsForce <= 1.6)
                tenzoStep = new Force(0.2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 1.6 && tenzoMaxValue.KilogramsForce <= 2.5)
                tenzoStep = new Force(0.5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 2.5 && tenzoMaxValue.KilogramsForce <= 4.0)
                tenzoStep = new Force(0.5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 4.0 && tenzoMaxValue.KilogramsForce <= 6.0)
                tenzoStep = new Force(1, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 6.0 && tenzoMaxValue.KilogramsForce <= 10.0)
                tenzoStep = new Force(2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 10.0 && tenzoMaxValue.KilogramsForce <= 16.0)
                tenzoStep = new Force(2, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 16.0 && tenzoMaxValue.KilogramsForce <= 25.0)
                tenzoStep = new Force(5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 25.0 && tenzoMaxValue.KilogramsForce <= 40.0)
                tenzoStep = new Force(5, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 40.0 && tenzoMaxValue.KilogramsForce <= 60.0)
                tenzoStep = new Force(10, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 60.0 && tenzoMaxValue.KilogramsForce <= 100.0)
                tenzoStep = new Force(20, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 100.0 && tenzoMaxValue.KilogramsForce <= 160.0)
                tenzoStep = new Force(20, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 160.0 && tenzoMaxValue.KilogramsForce <= 250.0)
                tenzoStep = new Force(50, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 250.0 && tenzoMaxValue.KilogramsForce <= 400.0)
                tenzoStep = new Force(50, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 400.0 && tenzoMaxValue.KilogramsForce <= 600.0)
                tenzoStep = new Force(100, UnitsNet.Units.ForceUnit.KilogramForce);
            else if (tenzoMaxValue.KilogramsForce > 600.0)
                tenzoStep = new Force(200, UnitsNet.Units.ForceUnit.KilogramForce);

            double yPressureAxisMinValue = pressureSensor.MinValue.ToUnit(bench.Settings.PressureUnit).Value;
            double yPressureAxisMaxValue = pressureSensor.MaxValue.ToUnit(bench.Settings.PressureUnit).Value * 1.1;
            yPressureAxis.VisibleRange = new DoubleRange(yPressureAxisMinValue, yPressureAxisMaxValue);
            yPressureAxis.MajorDelta = pressureStep.ToUnit(bench.Settings.PressureUnit).Value;
            yPressureAxis.MinorDelta = pressureStep.ToUnit(bench.Settings.PressureUnit).Value / 10.0;

            double yTenzoAxisMinValue = tenzoSensor.MinValue.ToUnit(bench.Settings.TenzoUnit).Value;
            double yTenzoAxisMaxValue = tenzoSensor.MaxValue.ToUnit(bench.Settings.TenzoUnit).Value * 1.1;
            yTenzoAxis.VisibleRange = new DoubleRange(yTenzoAxisMinValue, yTenzoAxisMaxValue);
            yTenzoAxis.MajorDelta = tenzoStep.ToUnit(bench.Settings.TenzoUnit).Value;
            yTenzoAxis.MinorDelta = tenzoStep.ToUnit(bench.Settings.TenzoUnit).Value / 10.0;
        }

        public ObservableCollection<IAnnotationViewModel> Annotations { get; set; } = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IRenderableSeriesViewModel> Series { get; set; } = new ObservableCollection<IRenderableSeriesViewModel>();
        public ObservableCollection<IAxisViewModel> XAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollection<IAxisViewModel> YAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
    }
}
