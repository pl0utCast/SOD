using DynamicData;
using SOD.App.Benches;
using SOD.App.Benches.SODBench;
using SOD.App.Testing.Standarts;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.LocalizationService;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using UnitsNet;

namespace SOD.App.Testing.Test
{
    public class Test : ITesting
    {
        private readonly BaseReportData _reportData;
        private readonly object[] _parameters;
        private readonly IStandart _standart;
        private List<Pressure> pressures = new List<Pressure>();
        private IDisposable pressureUpdaterDisposable;
        private List<Action> sensorValueUpdaters;
        private ITestBench _testBench;
        protected IEnumerable<IPost> _posts;
        private Timer timer;

        public Test(string name,
                    BaseReportData baseReportData,
                    ILocalizationService localizationService,
                    IStandart standart,
                    params object[] parameters)
        {
            Name = name;
            _reportData = baseReportData;
            _parameters = parameters;
            _standart = standart;
        }

        //public void CalculateResult()
        //{
        //          TestResult.Standart = _standart != null ? _standart.Name : string.Empty;

        //	//var expectedSetPressure = (Pressure)_parameters[0];
        //	//var slumpDiffPressure = ((Pressure)_parameters[2]).Bars;
        //	//var sensitivity = (int)_parameters[3];

        //	int slumpIndex = 0;
        //	int closingIndex = 0;
        //	var isSlump = false;
        //	if (pressures.Count > 0)
        //	{
        //		var data = pressures.Select(p => p.Bars).ToArray();
        //		isSlump = true;
        //		slumpIndex = FindMaximum(data, 0, data.Count() - 1);
        //		closingIndex = FindMinimum(data, 0, data.Count() - 1);
        //		//var minDP = double.MaxValue;
        //		//for (slumpIndex = 0; slumpIndex < pressures.Count && !isSlump; slumpIndex++)
        //		//{
        //		//	var dp = 0.0;

        //		//	for (var i = 0; i < sensitivity; i++)
        //		//	{
        //		//		var pressure = data[slumpIndex - i];
        //		//		var prevPressure = data[slumpIndex - i - 1];

        //		//		dp += pressure - prevPressure;

        //		//		isSlump = -dp > slumpDiffPressure;

        //		//		if (dp >= minDP) continue;

        //		//		minDP = dp;
        //		//		//DebugTools.DebugWrite("index = " + slumpIndex + "; ");
        //		//		//DebugTools.DebugWriteLine("dp = " + dp);
        //		//	}
        //		//}

        //		//if (isSlump)
        //		//{
        //		//	slumpIndex = FindMaximum(data, slumpIndex - sensitivity, slumpIndex);
        //		//	if (slumpIndex < 0) slumpIndex = 0;

        //		//	var isClosed = false;
        //		//	closingIndex = slumpIndex;
        //		//	var minSensitivity = 50;

        //		//	for (; closingIndex < pressures.Count && !isClosed; closingIndex++)
        //		//	{
        //		//		var dp = 0.0;

        //		//		for (var j = 1; j < minSensitivity && closingIndex + j < pressures.Count; j++)
        //		//		{
        //		//			var pressure = data[closingIndex + j];
        //		//			var prevPressure = data[closingIndex];

        //		//			dp += pressure - prevPressure;
        //		//		}

        //		//		isClosed = dp > 0F;
        //		//	}

        //		//	var start = closingIndex - minSensitivity;
        //		//	if (start < slumpIndex) start = slumpIndex;
        //		//	closingIndex = FindMinimum(data, start, closingIndex);
        //		//}
        //	}

        //	var postResult = new Result.PostResult();
        //	postResult.OpenPressure = isSlump ? pressures[slumpIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
        //	postResult.ClosePressure = isSlump ? pressures[closingIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
        //	//postResult.OpenPoint = isSlump ? slumpIndex : 0;
        //	//postResult.ClosePoint = isSlump ? closingIndex : 0;
        //	postResult.ExpectedSetPressure = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
        //          TestResult.PostResults.Add(postResult);
        //}

        public void CalculateResult()
        {
            TestResult.Clear();
            TestResult.Name = Name;
            TestResult.Standart = _standart != null ? _standart.Name : string.Empty;
            // на каждую регистрацию предполагается 2 маркера, старт и стоп
            foreach (var post in _testBench.Posts)
            {
                if (!post.IsEnable) continue;
                if (registrationMarkers.Count % 2.0 != 0) return;
                var postResult = new Result.PostResult();

                //postResult.OpenPressure = isSlump ? pressures[slumpIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
                //postResult.ClosePressure = isSlump ? pressures[closingIndex] : new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
                //postResult.OpenPoint = isSlump ? slumpIndex : 0;
                //postResult.ClosePoint = isSlump ? closingIndex : 0;
                //postResult.ExpectedSetPressure = expectedSetPressure;

                for (int i = 0; i < registrationMarkers.Count - 1; i += 2)
                {
                    var registration = new Result.Registration();
                    registration.Time = TimeSpan.FromSeconds((registrationMarkers[i + 1] - registrationMarkers[i]) / 10.0); // 100 мсек время одного тика в испытании
                    foreach (var benchSensor in post.Sensors)
                    {
                        if (benchSensor.Sensor is IPressureSensor pressureSensor)
                        {
                            var startPressure = pressures[registrationMarkers[i]]; // маркер старта
                            var stopPressure = pressures[registrationMarkers[i + 1]]; // маркер остановки
                            var diffPressure = startPressure - stopPressure;
                            registration.StartPressure.Add(new SensorResultValue<Pressure>(pressureSensor.Id, pressureSensor.Name, startPressure));
                            registration.StopPressure.Add(new SensorResultValue<Pressure>(pressureSensor.Id, pressureSensor.Name, stopPressure));
                            registration.DropPressure.Add(new SensorResultValue<Pressure>(pressureSensor.Id, pressureSensor.Name, diffPressure));

                            if (post.Status == PostStatus.Valid)
                                registration.Result = "Соответствует";
                            else
                                registration.Result = "Не соответствует";

                            postResult.Registrations.Add(registration);
                        }
                    }
                }

                TestResult.PostResults.Add(postResult);
            }
        }

        public void Dispose()
        {
            TestResult?.Clear();
        }

        public void FillReport(Bitmap chartImage)
        {
            if (chartImage != null)
            {
                TestResult.Chart = chartImage;
                //if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png")))
                //{
                //	using (var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png"), FileMode.Open))
                //	{
                //		TestResult.QrChart = new Bitmap(fileStream);
                //	}
                //}
            }
            _reportData?.Fill(TestResult);
        }

        public void Start(ITestBench testBench)
        {
            if (IsRun) return;
            IsRun = true;
            TestResult.Clear();
            _testBench = testBench;
            var sensor = ((Benches.SODBench.Bench)testBench).Sensors.Where(s => s.Sensor is IPressureSensor).FirstOrDefault();
            pressureUpdaterDisposable = ((IPressureSensor)sensor?.Sensor).Subscribe(
                pressure =>
                {
                    pressures.Add(pressure);
                });
        }

        public void StartCollectData()
        {
            sensorValueUpdaters = new List<Action>();
            var sensor = ((Benches.SODBench.Bench)_testBench).Sensors.Where(s => s.Sensor is IPressureSensor).FirstOrDefault();

            if (sensor.Sensor is IPressureSensor pressureSensor)
            {
                //pressures.Add(pressureSensor.Id, new List<Pressure>());
                sensorValueUpdaters.Add(() =>
                {
                    pressures.Add(pressureSensor.Pressure.ToUnit(_testBench.Settings.PressureUnit));
                });
                /*pressureSensor.Subscribe(p =>
				{
					var press = p.ToUnit(_testBench.Settings.PressureUnit);
					pressures[pressureSensor.Id].Add(press);
				}).DisposeWith(disposables);*/
            }

            timer?.Dispose();
            // таймер тикает каждые 100 мс
            timer = new Timer(c =>
                {
                    foreach (var updater in sensorValueUpdaters)
                    {
                        updater();
                    }
                }, null, 0, 100);

        }

        public void StartRegistration()
        {
            registrationMarkers.Add(pressures.Count - 1);
        }

        public void Stop()
        {
            if (!IsRun) return;
            IsRun = false;
            pressureUpdaterDisposable?.Dispose();
        }

        public void StopRegistration()
        {
            registrationMarkers.Add(pressures.Count - 1);
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
        protected List<int> registrationMarkers { get; set; } = new List<int>();
        public TestType TestType => TestType.Functional;
        public ITestingResult Result => TestResult;
        public string Name { get; set; }
        public bool IsRun { get; private set; }
    }
}
