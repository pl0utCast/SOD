using SOD.Core.Balloons;
using SOD.Core.Infrastructure;
using SOD.Core.Props;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.LocalizationService;
using System.Data;
using System.Globalization;
using UnitsNet;

namespace SOD.App.Benches.SODBench.Report
{
    public class ReportData : BaseReportData
    {
        private readonly ISensorService sensorService;
        private readonly Bench bench;

        public ReportData(ISensorService sensorService, Bench bench)
        {
            this.sensorService = sensorService;
            this.bench = bench;
        }

        public override void Fill(params object[] parameters)
        {
            if (parameters[0] is List<SOD.Core.Balloons.Properties.BalloonProperty> balloon)
            {
                FillBalloon(balloon);
            }
            else if (parameters[0] is IEnumerable<Property> props)
            {
                FillParameters(props);
            }
            else if (parameters[0] is Testing.Test.Result testResult)
            {
                Fill(testResult);
            }
        }

        private void Fill(Testing.Test.Result result)
        {
            var test = new TestReportItem();
            test.Name = result.Name;
            test.Chart = result.Chart;
            //if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
            //	test.QrChart = result.QrChart;
            test.Standart = result.Standart;
            foreach (var postResult in result.PostResults)
            {
                var reportPost = new PostDataReport();
                foreach (var registration in postResult.Registrations)
                {
                    var pressureSensor = (IPressureSensor)sensorService.GetSensor(registration.StartPressure.First().Id);
                    Pressure startPressure = new Pressure(0, registration.StartPressure[0].Value.Unit);
                    Pressure stopPressure = new Pressure(0, registration.StopPressure[0].Value.Unit);
                    Pressure dropPressure = new Pressure(0, registration.DropPressure[0].Value.Unit);

                    for (int i = 0; i < registration.StartPressure.Count; i++)
                    {
                        startPressure += registration.StartPressure[i].Value;
                        stopPressure += registration.StopPressure[i].Value;
                        dropPressure += registration.DropPressure[i].Value;
                    }
                    startPressure = startPressure / registration.StartPressure.Count;
                    stopPressure = stopPressure / registration.StartPressure.Count;
                    dropPressure = dropPressure / registration.StartPressure.Count;

                    var reportRegistration = new PostDataReport.Registration();

                    // Протечка не используется, но мб потом пригодится
                    if (registration.Leakage.Count > 0)
                    {
                        var leakageSensor = (ILeakageSensor)sensorService.GetSensor(registration.Leakage.First().Id);
                        reportRegistration.Leakage = registration.Leakage[0].Value.ToString(leakageSensor.Accaury);
                    }

                    reportRegistration.Time = registration.Time;
                    reportRegistration.StartPressure = startPressure.ToString(pressureSensor.Accaury);
                    reportRegistration.StopPressue = stopPressure.ToString(pressureSensor.Accaury);
                    reportRegistration.DropPressure = dropPressure.ToString(pressureSensor.Accaury);
                    reportRegistration.PressureName = pressureSensor.Name;
                    reportRegistration.Result = registration.Result;
                    reportPost.Registrations.Add(reportRegistration);
                }

                test.Posts.Add(reportPost);
            }
            MainData.Tests.Add(test);
        }

        public override List<KeyValuePair<string, object>> GetData()
        {
            var result = new List<KeyValuePair<string, object>>();
            result.Add(new KeyValuePair<string, object>("Balloon", TestingBalloon));
            result.Add(new KeyValuePair<string, object>("MainData", MainData));
            result.Add(new KeyValuePair<string, object>("Parameters", Parameters));
            return result;
        }

        private void FillBalloon(List<SOD.Core.Balloons.Properties.BalloonProperty> balloon)
        {
            TestingBalloon = new ReportBalloon();

            foreach (var property in balloon)
            {
                if (property.Type == PropertyType.StringList)
                {
                    var list = (IList<object>)property.Value;
                    TestingBalloon.Properties.Add(new BalloonProperty(property.Name, (string)list.Last()));
                }
                else
                    TestingBalloon.Properties.Add(new BalloonProperty(property.Name, property.Value?.ToString()));
            }
        }

        private void FillParameters(IEnumerable<Property> properties)
        {
            Parameters = new DataSet();
            var table = Parameters.Tables.Add("Parameters");
            List<object> templist = new List<object>();
            foreach (var property in properties)
            {
                table.Columns.Add(property.Alias);
                if (property.Type == PropertyType.StringList)
                {
                    templist.Add(((IList<object>)property.Value).Last());
                }
                else
                    templist.Add(property.Value.ToString());
            }
            table.Rows.Add(templist.ToArray());
        }
        public ReportBalloon TestingBalloon { get; set; }
        public TestReportData MainData { get; set; } = new TestReportData();
        public DataSet Parameters { get; set; }
        public override bool IsFill => MainData.Tests.Count > 0 ? true : false;

        public class BalloonProperty
        {
            public BalloonProperty(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ReportBalloon
        {
            public List<BalloonProperty> Properties { get; set; } = new List<BalloonProperty>();
        }
    }
}
