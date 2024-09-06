using MemBus;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.Core.Valves;
using SOD.App.Benches;
using SOD.App.Messages.Test;
using SOD.App.Testing.Leakage;
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

namespace SOD.App.Testing.Leakage
{
    public class Test : BaseTest
    {
        private readonly BaseReportData _reportData;
        private readonly IStandart _standart;
        private TimeSpan registrationTime;
        public Test(string name,
                    BaseReportData reportData,
                    ILocalizationService localizationService,
                    IStandart standart = null,
                    params object[] parameters) : base(localizationService,
                                                       standart,
                                                       parameters)
        {
            Name = name;
            _reportData = reportData;
            _standart = standart;
        }
        public override void CalculateResult()
        {
            TestResult.Clear();
            //TestResult.Chart = CreateChart();
            TestResult.Standart = _standart != null ? _standart.Name : string.Empty;
            TestResult.Medium = _mediumType;
            if (registrationMarkers.Count % 2.0 != 0) return;
            // на каждую регистрацию предпологается 2 маркера, старт и стоп
            foreach (var post in _posts)
            {
                if (!post.IsEnable) continue;
                var postResult = new Result.PostResult();
                postResult.PostId = post.Id;
                postResult.SerialNumber = post.SerialNumber;


                for (int i = 0; i < registrationMarkers.Count-1; i+=2)
                {
                    var registration = new Result.Registration();
                    registration.Time = TimeSpan.FromSeconds((registrationMarkers[i + 1] - registrationMarkers[i])/10.0); // 100 мсек время одного тика в испытании

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
                        }
                        if (benchSensor.Sensor is ILeakageSensor leakageSensor)
                        {
                            var sensorLeakages = leakages[leakageSensor.Id];
                            var leakage = sensorLeakages[registrationMarkers[i+1]];
                            registration.Leakage.Add(new SensorResultValue<UnitsNet.VolumeFlow>(leakageSensor.Id, leakageSensor.Name, leakage));
                            if (_standart == null && _parameters[0] is ControlType controlType)
                            {
                                switch (controlType)
                                {
                                    case ControlType.Leakage:
                                        {
                                            if (_parameters[1] is VolumeFlow entryLeakage)
                                            {
                                                if (leakage <= entryLeakage) registration.Result = localizationService["Testing.Test.Valid"];
                                                else registration.Result = localizationService["Testing.Test.UnValid"];
                                            }
                                            break;
                                        }
                                    case ControlType.Manual:
                                        {
                                            if (post.Status == PostStatus.Valid)
                                                registration.Result = localizationService["Testing.Test.Valid"];
                                            else
                                            {
                                                _posts.SingleOrDefault(p => p.Id == postResult.PostId).IsEnable = false;
                                                registration.Result = localizationService["Testing.Test.UnValid"];
                                            }
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                // средние давление по всем датчикам доступным в испытании
                                var testPressure = new Pressure(registration.StopPressure.Average(p => p.Value.Bars), UnitsNet.Units.PressureUnit.Bar); 
                                var result = _standart.Calculate(_valve, _mediumType, leakage, testPressure);
                                if (result == null) registration.Result = localizationService["Testing.Test.UnValid"];
                                else
                                {
                                    if (!result.IsValid) registration.Result = localizationService["Testing.Test.UnValid"];
                                    else
                                    {
                                        if (result.Info == null) registration.Result = localizationService["Testing.Test.Valid"];
                                        else registration.Result = $"{localizationService["Testing.Test.Class"]} {result.Info}";
                                    }
                                }
                            }
                        }    
                    }
                    postResult.Registrations.Add(registration);
                }
                
                
                TestResult.PostResults.Add(postResult);
            }
        }

        public override void FillReport( Bitmap chartImage = null)
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

        public override void Dispose()
        {
            TestResult.Clear();
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

        public override TestType TestType => TestType.Leakage;

        public override ITestingResult Result => TestResult;
    }
}
