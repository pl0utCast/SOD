using SOD.Core.Infrastructure;
using SOD.Core.Props;
using SOD.Core.Reports;
using SOD.Core.Sensor;
using SOD.Core.Valves;
using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnitsNet;

namespace SOD.App.Benches.CRSBench.Report
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
            if (parameters[0] is Valve valve)
            {
                FillValve(valve);
            }
            else if (parameters[0] is IEnumerable<Property> props)
            {
                FillParameters(props);
            }
            //else if (parameters[0] is string && parameters[1] is Testing.Strength.Result strenghtResult)
            //{
            //    Fill((string)parameters[0], strenghtResult);
            //}
            //else if (parameters[0] is string && parameters[1] is Testing.Leakage.Result leakageResult)
            //{
            //    Fill((string)parameters[0], leakageResult);
            //}
            //else if (parameters[0] is string && parameters[1] is Testing.Funcional.Result funcionalResult)
            //{
            //    Fill((string)parameters[0], funcionalResult);
            //}
        }

        private void Fill(string name)
        {
            var test = new TestReportItem();
            test.Name = name;
            //test.Chart = strenghtResult.Chart;
            //if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
            //    test.QrChart = strenghtResult.QrChart;
            //test.Standart = strenghtResult.Standart;
            //test.Medium = strenghtResult.Medium.ToString();
            //foreach (var postResult in strenghtResult.PostResults)
            //{
            //    var reportPost = new PostDataReport();
            //    reportPost.PostNumber = postResult.PostId;
            //    reportPost.SerialNumber = postResult.SerialNumber;
            //    foreach (var registration in postResult.Registrations)
            //    {
            //        var pressureSensor = (IPressureSensor)sensorService.GetSensor(registration.StartPressure.First().Id); 
            //        Pressure startPressure = new Pressure(0, registration.StartPressure[0].Value.Unit);
            //        Pressure stopPressure = new Pressure(0, registration.StopPressure[0].Value.Unit);
            //        Pressure dropPressure = new Pressure(0, registration.DropPressure[0].Value.Unit);

            //        for (int i = 0; i < registration.StartPressure.Count; i++)
            //        {
            //            startPressure += registration.StartPressure[i].Value;
            //            stopPressure += registration.StopPressure[i].Value;
            //            dropPressure += registration.DropPressure[i].Value;
            //        }
            //        startPressure = startPressure / registration.StartPressure.Count;
            //        stopPressure = stopPressure / registration.StartPressure.Count;
            //        dropPressure = dropPressure / registration.StartPressure.Count;

            //        var reportRegistration = new PostDataReport.Registration();
            //        reportRegistration.Time = registration.Time;
            //        reportRegistration.StartPressure = startPressure.ToString(pressureSensor.Accaury);
            //        reportRegistration.StopPressue = stopPressure.ToString(pressureSensor.Accaury);
            //        reportRegistration.DropPressure = dropPressure.ToString(pressureSensor.Accaury);
            //        reportRegistration.PressureName = pressureSensor.Name;
            //        reportRegistration.Result = registration.Result;
            //        reportPost.Registrations.Add(reportRegistration);
            //    }
                
            //    test.Posts.Add(reportPost);

            //}
            MainData.Tests.Add(test);
        }

        //private void Fill(string name)
        //{
        //    var test = new TestReportItem();
        //    test.Name = name;
        //    test.Chart = result.Chart;
        //    if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
        //        test.QrChart = result.QrChart;
        //    test.Standart = result.Standart;
        //    foreach (var postResult in result.PostResults)
        //    {
        //        var reportPost = new PostDataReport();
        //        reportPost.PostNumber = postResult.PostId;
        //        reportPost.SerialNumber = postResult.SerialNumber;
        //        foreach (var registration in postResult.Registrations)
        //        {
        //            var pressureSensor = (IPressureSensor)sensorService.GetSensor(registration.StartPressure.First().Id);
        //            var leakageSensor = (ILeakageSensor)sensorService.GetSensor(registration.Leakage.First().Id);
        //            Pressure startPressure = new Pressure(0, registration.StartPressure[0].Value.Unit);
        //            Pressure stopPressure = new Pressure(0, registration.StopPressure[0].Value.Unit);
        //            Pressure dropPressure = new Pressure(0, registration.DropPressure[0].Value.Unit);

        //            for (int i = 0; i < registration.StartPressure.Count; i++)
        //            {
        //                startPressure += registration.StartPressure[i].Value;
        //                stopPressure += registration.StopPressure[i].Value;
        //                dropPressure += registration.DropPressure[i].Value;
        //            }
        //            startPressure = startPressure / registration.StartPressure.Count;
        //            stopPressure = stopPressure / registration.StartPressure.Count;
        //            dropPressure = dropPressure / registration.StartPressure.Count;

        //            var reportRegistration = new PostDataReport.Registration();
        //            reportRegistration.Time = registration.Time;
        //            reportRegistration.StartPressure = startPressure.ToString(pressureSensor.Accaury);
        //            reportRegistration.StopPressue = stopPressure.ToString(pressureSensor.Accaury);
        //            reportRegistration.DropPressure = dropPressure.ToString(pressureSensor.Accaury);
        //            reportRegistration.PressureName = pressureSensor.Name;
        //            reportRegistration.LeakageTest.Leakage = registration.Leakage[0].Value.ToString(leakageSensor.Accaury);

        //            var gasSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.GasTemperatureSensorId);
        //            reportRegistration.AirTemperature = ((ITemperatureSensor)gasSensor).Temperature.ToString(gasSensor.Accaury);

        //            //var liquidSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.LiquidTemperatureSensorId);
        //            //reportRegistration.WaterTemperature = ((ITemperatureSensor)liquidSensor).Temperature.ToString(liquidSensor.Accaury);

        //            reportRegistration.Result = registration.Result;
        //            reportPost.Registrations.Add(reportRegistration);
        //        }    
        //        test.Posts.Add(reportPost);
        //    }
        //    MainData.Tests.Add(test);
        //}

        //private void Fill(string name, Testing.Funcional.Result result)
        //{
        //    var test = new TestReportItem();
        //    test.Name = name;
        //    test.Chart = result.Chart;
        //    if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
        //        test.QrChart = result.QrChart;
        //    test.Standart = result.Standart;
        //    foreach (var postResult in result.PostResults)
        //    {
        //        var reportPost = new PostDataReport();
        //        var reportRegistration = new PostDataReport.Registration();
        //        reportRegistration.FuncionalTest.OpenPressure = postResult.OpenPressure.ToString("f2");
        //        reportRegistration.FuncionalTest.ClosePressure = postResult.ClosePressure.ToString("f2");
        //        reportRegistration.FuncionalTest.OpenPressureAccuracy = postResult.Accuracy.ToString("f1");
        //        reportRegistration.FuncionalTest.ExpectedSetPressure = postResult.ExpectedSetPressure.ToString("f2");

        //        var gasSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.GasTemperatureSensorId);
        //        reportRegistration.AirTemperature = ((ITemperatureSensor)gasSensor).Temperature.ToString(gasSensor.Accaury);

        //        //var liquidSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.LiquidTemperatureSensorId);
        //        //reportRegistration.WaterTemperature = ((ITemperatureSensor)liquidSensor).Temperature.ToString(liquidSensor.Accaury);

        //        reportRegistration.Result = postResult.Result;
        //        reportPost.Registrations.Add(reportRegistration);
        //        test.Posts.Add(reportPost);
        //    }
        //    MainData.Tests.Add(test);
        //}

        public override List<KeyValuePair<string, object>> GetData()
        {
            var result = new List<KeyValuePair<string, object>>();
            result.Add(new KeyValuePair<string, object>("Valve", TestingValve));
            result.Add(new KeyValuePair<string, object>("MainData", MainData));
            result.Add(new KeyValuePair<string, object>("Parameters", Parameters));
            return result;
        }

        private void FillValve(Valve valve)
        {
            /*TestingValve = new DataSet();
            var table = TestingValve.Tables.Add("Valve");
            table.Columns.Add(nameof(valve.Name));
            foreach (var property in valve.Properties)
            {
                table.Columns.Add(property.Prefix);
            }
            var values = new List<object>();
            values.Add(valve.Name);
            values.AddRange(valve.Properties.Select(p => p.Value?.ToString()));
            table.Rows.Add(values.ToArray());*/
            TestingValve = new ReportValve();
            foreach (var property in valve.Properties)
            {
                if (property.Type == PropertyType.StringList)
                {
                    var list = (IList<object>)property.Value;
                    TestingValve.Properties.Add(new ValveProperty(property.Name, (string)list.Last()));
                }
                else
                    TestingValve.Properties.Add(new ValveProperty(property.Name, property.Value?.ToString()));
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
        public ReportValve TestingValve { get; set; }
        public TestReportData MainData { get; set; } = new TestReportData();
        public DataSet Parameters { get; set; }

        public override bool IsFill => MainData.Tests.Count > 0 ? true : false;

        public class ValveProperty
        {
            public ValveProperty(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ReportValve
        {
            public List<ValveProperty> Properties { get; set; } = new List<ValveProperty>();
        }
    }
}
