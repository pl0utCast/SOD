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

        public PressureChartViewModel(ILocalizationService localizationService, SOD.App.Benches.SODBench.Bench bench)
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
                IsEditable = false
            });
            ConfigureAxis();
            
        }

        public void ConfigureAxis()
        {
            // конфигурация Axes
            var xTimeSpanAxis = new TimeSpanAxisViewModel
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
                AxisTitle = localizationService["Testing.SODBench.Pressure"],
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
                BorderBrush=System.Windows.Media.Brushes.Green
            };
            YAxes.Add(yPressureAxis);
        }

        public void SetPressureSensor(IPressureSensor pressureSensor)
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
                PressureSeries.Add(new LineRenderableSeriesViewModel() { DataSeries = pressSeries, AntiAliasing = true, Stroke = Colors.Red, StrokeThickness=2 });
                pressureSeries.Add(pressureSensor.Id, pressSeries);
            }

            yPressureAxis.AxisTitle = localizationService["Testing.SODBench.Pressure"] +
                " ("+UnitsNet.UnitAbbreviationsCache.Default.GetDefaultAbbreviation(typeof(PressureUnit), (int)bench.Settings.PressureUnit)+")";
        }

        public void StartChart()
        {
            if (isStartChart) return;

            pressureUpdater = Observable.Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
                                        .Subscribe(time =>
                                        {
                                            totalTime = TimeSpan.FromMilliseconds(time*100);
                                            foreach (var pressureSensor in pressureSensors.ToList())
                                            {
                                                double currentPressure = Math.Round(pressureSensor.Pressure.ToUnit(bench.Settings.PressureUnit).Value, 2);

                                                if (pressureSensor.Pressure.ToUnit(PressureUnit.Bar) > Pressure.FromBars(100) && (pressureSensor.Id == 16 || pressureSensor.Id == 17)) //при давлении больше 100 бар переключаем на второй датчик
                                                {
                                                    bench.Settings.SelectedTestSettings.PressureSensorId = 17; //переключение на датчик давления id=17
                                                    //bench.UpdatePosts();
                                                    bench.SaveSettings();

                                                    if (PressureSeries.Count <= 1)
                                                    {
                                                        //foreach (var post in bench.Posts)
                                                        //{
                                                        //    foreach (var pSensor in post.Sensors.Where(s => s.Sensor is IPressureSensor).Select(s => s.Sensor))
                                                        //    {
                                                        //        Application.Current.Dispatcher.Invoke((Action)delegate
                                                        //        {
                                                        //            SetPressureSensor((IPressureSensor)pSensor);
                                                        //        });
                                                        //    }
                                                        //}
                                                    }

                                                    if (pressureSensor.Id == 17)
                                                    {
                                                        PressureSeries[1].Stroke = Colors.Blue;
                                                        pressureSeries[pressureSensor.Id].Append(totalTime, currentPressure);
                                                    }
                                                }
                                                else
                                                {
                                                    if (pressureSensor.Id == 20 && PressureSeries.Count > 1)
                                                    {
                                                        PressureSeries[1].Stroke = Colors.Blue;
                                                        pressureSeries[pressureSensor.Id].Append(totalTime, currentPressure);
                                                    }
                                                    else if (pressureSensor.Id != 17)
                                                    {
                                                        PressureSeries[0].Stroke = Colors.Red;
                                                        pressureSeries[pressureSensor.Id].Append(totalTime, currentPressure);
                                                    }

                                                    if (bench.Settings.SelectedTestSettings.PressureSensorId == 17)
                                                    {
                                                        bench.Settings.SelectedTestSettings.PressureSensorId = 16; //переключение на датчик давления id=16

                                                        //bench.UpdatePosts();
                                                        bench.SaveSettings();
                                                    }
                                                }

                                                if (bench.Settings.AutoRange)
                                                    yPressureAxis.VisibleRange = new DoubleRange(PressureSeries[0].DataSeries.YMin.ToDouble() - Math.Abs(PressureSeries[0].DataSeries.YMin.ToDouble() * 5 / 100), PressureSeries[0].DataSeries.YMax.ToDouble() + Math.Abs(PressureSeries[0].DataSeries.YMax.ToDouble() * 5 / 100));
                                            }
                                        });
            isStartChart = true;
        }

        public void StopChart()
        {
            if (!isStartChart) return;

            if (bench.Settings.SelectedTestSettings.PressureSensorId == 16 || bench.Settings.SelectedTestSettings.PressureSensorId == 17)
            {
                bench.Settings.SelectedTestSettings.PressureSensorId = 16; //переключение на датчик давления id=16
                //bench.UpdatePosts();
                bench.SaveSettings();
            }

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
                IsEditable = false
            });      
        }

        public ObservableCollection<IAnnotationViewModel> Annotations { get; set; } = new ObservableCollection<IAnnotationViewModel>();
        public ObservableCollection<IRenderableSeriesViewModel> PressureSeries { get; set; } = new ObservableCollection<IRenderableSeriesViewModel>();
        public ObservableCollection<IAxisViewModel> XAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
        public ObservableCollection<IAxisViewModel> YAxes { get; set; } = new ObservableCollection<IAxisViewModel>();
    }
}
