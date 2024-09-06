using SOD.App.Benches;
using SOD.App.Testing.Standarts;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.LocalizationService;
using System.Drawing;
using System.IO;
using UnitsNet;

namespace SOD.App.Testing.Strength
{
    public class Test : BaseTest
    {
        private readonly BaseReportData _reportData;
        private readonly IStandart _standart;
        public Test(string name,
                    BaseReportData reportData,
                    ILocalizationService localizationService,
                    IStandart standart = null,
                    params object[] parameters) : base(localizationService, standart, parameters)
        {
            Name = name;
            _reportData = reportData;
            _standart = standart;
        }
        public override void CalculateResult()
        {
            TestResult.Clear();
            TestResult.Standart = _standart != null ? _standart.Name : string.Empty;
            TestResult.Medium = _mediumType;
            // на каждую регистрацию предпологается 2 маркера, старт и стоп
            foreach (var post in _posts)
            {
                if (!post.IsEnable) continue;
                if (registrationMarkers.Count % 2.0 != 0) return;
                var postResult = new Result.PostResult();
                postResult.PostId = post.Id;
                postResult.SerialNumber = post.SerialNumber;
                

                for (int i = 0; i < registrationMarkers.Count - 1; i += 2)
                {
                    var registration = new Result.Registration();
                    registration.Time = TimeSpan.FromSeconds((registrationMarkers[i + 1] - registrationMarkers[i]) / 10.0); // 100 мсек время одного тика в испытании
                    foreach (var benchSensor in post.Sensors)
                    {
                        if (benchSensor.Sensor is IPressureSensor pressureSensor)
                        {
                            List<Pressure> sensorPressures;
                            if (pressures.ContainsKey(16)) //для автоматического переключения датчика давления
                                sensorPressures = pressures[16];
                            else
                                sensorPressures = pressures[pressureSensor.Id];

                            var startPressure = sensorPressures[registrationMarkers[i]]; // маркер старта
                            var stopPressure = sensorPressures[registrationMarkers[i+1]]; // маркер остановки
                            var diffPressure = startPressure - stopPressure;
                            registration.StartPressure.Add(new SensorResultValue<UnitsNet.Pressure>(pressureSensor.Id, pressureSensor.Name, startPressure));
                            registration.StopPressure.Add(new SensorResultValue<UnitsNet.Pressure>(pressureSensor.Id, pressureSensor.Name, stopPressure));
                            registration.DropPressure.Add(new SensorResultValue<UnitsNet.Pressure>(pressureSensor.Id, pressureSensor.Name, diffPressure));

                            // оцениваем результат по параметрам заданным оператором
                            if (_parameters[0] is ControlType controlType)
                            {
                                switch (controlType)
                                {
                                    case ControlType.Manometric:
                                        {
                                            if (_parameters[1] is double pressurePercent)
                                            {
                                                if (diffPressure <= (stopPressure / 100.0) * pressurePercent)
                                                {
                                                    
                                                    registration.Result = "Соответствует";
                                                }
                                                else
                                                {
                                                    post.IsEnable = false;
                                                    registration.Result = "Не соответствует";
                                                }
                                            }
                                            break;
                                        }
                                    case ControlType.Manual:
                                        {
                                            if (post.Status == PostStatus.Valid)
                                                registration.Result = "Соответствует";
                                            else
                                            {
                                                registration.Result = "Не соответствует";
                                            }
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }

                            postResult.Registrations.Add(registration);
                        }
                    } 
                }
                
                TestResult.PostResults.Add(postResult);
            }
            
            
        }

        public override void Dispose()
        {
            TestResult.Clear();
        }

        public override void FillReport(Bitmap chartImage=null)
        {
            if (chartImage!=null)
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

        public override void StartRegistration()
        {
            registrationMarkers.Add(pressures.First().Value.Count);
        }

        public override void StopRegistration()
        {
            registrationMarkers.Add(pressures.First().Value.Count);  
        }

        public Result TestResult { get; set; } = new Result();

        public override TestType TestType => TestType.Strength;

        public override ITestingResult Result => TestResult;
    }
}
