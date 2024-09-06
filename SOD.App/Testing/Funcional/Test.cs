using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.App.Benches;
using SOD.App.Mediums;
using SOD.App.Testing.Standarts;
using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet;

namespace SOD.App.Testing.Funcional
{
    public class Test : ITesting
    {
        private readonly BaseReportData _reportData;
        private readonly object[] _parameters;
        private readonly IStandart _standart;
        private List<Pressure> pressures = new List<Pressure>();
        private IDisposable pressureUpdaterDisposable;
        public Test(string name,
                    BaseReportData baseReportData,
                    ILocalizationService localizationService,
                    IStandart standart,
                    params object[] parameters)
        {
            _reportData = baseReportData;
            Name = name;
            _parameters = parameters;
            _standart = standart;
        }

        public void CalculateResult()
        {
            TestResult.Standart = _standart != null ? _standart.Name : string.Empty;

            var expectedSetPressure = (Pressure)_parameters[0];
            var slumpDiffPressure = ((Pressure)_parameters[2]).Bars;
            var sensitivity = (int)_parameters[3];

            int slumpIndex = 0;
            int closingIndex = 0;
            var isSlump = false;
            if (pressures.Count > sensitivity)
            {
                var data = pressures.Select(p => p.Bars).ToArray();

                var minDP = double.MaxValue;
                for (slumpIndex = sensitivity; slumpIndex < pressures.Count && !isSlump; slumpIndex++)
                {
                    var dp = 0.0;

                    for (var i = 0; i < sensitivity; i++)
                    {
                        var pressure = data[slumpIndex - i];
                        var prevPressure = data[slumpIndex - i - 1];

                        dp += pressure - prevPressure;

                        isSlump = -dp > slumpDiffPressure;

                        if (dp >= minDP) continue;

                        minDP = dp;
                        //DebugTools.DebugWrite("index = " + slumpIndex + "; ");
                        //DebugTools.DebugWriteLine("dp = " + dp);
                    }
                }

                if (isSlump)
                {
                    slumpIndex = FindMaximum(data, slumpIndex - sensitivity, slumpIndex);
                    if (slumpIndex < 0) slumpIndex = 0;

                    var isClosed = false;
                    closingIndex = slumpIndex;
                    var minSensitivity = 50;

                    for (; closingIndex < pressures.Count && !isClosed; closingIndex++)
                    {
                        var dp = 0.0;

                        for (var j = 1; j < minSensitivity && closingIndex + j < pressures.Count; j++)
                        {
                            var pressure = data[closingIndex + j];
                            var prevPressure = data[closingIndex];

                            dp += pressure - prevPressure;
                        }

                        isClosed = dp > 0F;
                    }

                    var start = closingIndex - minSensitivity;
                    if (start < slumpIndex) start = slumpIndex;
                    closingIndex = FindMinimum(data, start, closingIndex);
                }
            }

            var postResult = new Result.PostResult();
            postResult.OpenPressure = isSlump ? pressures[slumpIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
            postResult.ClosePressure = isSlump ? pressures[closingIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
            postResult.OpenPoint = isSlump ? slumpIndex : 0;
            postResult.ClosePoint = isSlump ? closingIndex : 0;
            postResult.ExpectedSetPressure = expectedSetPressure;
            TestResult.PostResults.Add(postResult);
        }

        public void Dispose()
        {
            TestResult?.Clear();
        }

        public void FillReport(Bitmap chartImage = null)
        {
            if (chartImage != null)
            {
                TestResult.Chart = chartImage;
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png")))
                {
                    using (var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png"), FileMode.Open))
                    {
                        TestResult.QrChart = new Bitmap(fileStream);
                    }
                }
            }
            _reportData.Fill(Name, TestResult);
        }

        public void StartRegistration()
        {
        }

        public void StopRegistration()
        {
        }

        public void StartCollectData()
        {
            
        }

        public void Start(ITestBench testBench, MediumType mediumType)
        {
            if (IsRun) return;
            IsRun = true;
            TestResult.Clear();
            foreach (var post in testBench.Posts)
            {
                var sensor = post.Sensors.Where(s => s.Sensor is IPressureSensor).FirstOrDefault();
                pressureUpdaterDisposable = ((IPressureSensor)sensor.Sensor).Subscribe(
                    pressure =>
                    {
                        pressures.Add(pressure);
                    });
            }
        }

        public void Stop()
        {
            if (!IsRun) return;
            IsRun = false;
            pressureUpdaterDisposable?.Dispose();
        }

        public object[] GetTestParameters()
        {
            throw new NotImplementedException();
        }

        private static int FindMinimum(double[] data, int start, int finish)
        {
            if (start < 0) start = 0;
            var minValue = double.MaxValue;
            var minIndex = start;
            for (var i = start; i < finish; i++)
            {
                if (data[i] > minValue) continue;

                minIndex = i;
                minValue = data[i];
            }

            return minIndex;
        }

        private static int FindMaximum(double[] data, int start, int finish)
        {
            if (start < 0) start = 0;
            var maxValue = double.MinValue;
            var maxIndex = start;
            for (var i = start; i < finish; i++)
            {
                if (data[i] < maxValue) continue;

                maxIndex = i;
                maxValue = data[i];
            }

            return maxIndex;
        }

        public Result TestResult { get; set; } = new Result();
        public TestType TestType => TestType.Functional;

        public ITestingResult Result => TestResult;

        public bool IsRun { get; private set; }
        public string Name { get; set; }
    }
}
