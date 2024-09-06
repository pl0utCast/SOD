using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SOD.App.Benches.CRSBench.Report
{
    public class TestReportData
    {
        public int ReportNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public List<TestReportItem> Tests { get; set; } = new List<TestReportItem>();
    }

    public class TestReportItem
    {
        public string Name { get; set; }
        public Image Chart { get; set; }
        public Image QrChart { get; set; }
        public string CamerasDirectory { get; set; }
        public string Standart { get; set; }
        public string Medium { get; set; }
        public List<PostDataReport> Posts { get; set; } = new List<PostDataReport>();
    }

    public class PostDataReport
    {
        public List<Registration> Registrations { get; set; } = new List<Registration>();
        public int PostNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        

        public class Registration
        {
            public TimeSpan Time { get; set; }
            public string StartPressure { get; set; }
            public string StopPressue { get; set; }
            public string DropPressure { get; set; }
            public string PressureName { get; set; }
            public string AirTemperature { get; set; }
            public string WaterTemperature { get; set; }
            public LeakageReportTest LeakageTest { get; set; } = new LeakageReportTest();
            public FuncionalReportTest FuncionalTest { get; set; } = new FuncionalReportTest();
            public string Result { get; set; }

            public class LeakageReportTest
            {
                public string Leakage { get; set; }
                public string LeakageDrops { get; set; }
            }

            public class FuncionalReportTest
            {
                public string OpenPressure { get; set; }
                public string ClosePressure { get; set; }
                public string ExpectedSetPressure { get; set; }
                public string OpenPressureAccuracy { get; set; }
            }
        }
    }
}
