using SOD.Core.Props;
using SOD.Core.Settings;
using SOD.App.Mediums;
using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.App.Benches.CRSBench
{
    [ApplicationSettings]
    public class Settings : IBenchSettings
    {
        public Settings()
        {
            Tests.Add(1, new List<TestSettings>() { new TestSettings() { Name = "На прочность и плотность", Type = TestType.Strength } });
        }
        public int? SelectedValveTypeId { get; set; }
        public TestSettings SelectedTestSettings { get; set; }
        public PressureUnit PressureUnit { get; set; }
        public VolumeFlowUnit LeakageUnit { get; set; }
        public List<Property> Parameters { get; set; } = new List<Property>();
        public Dictionary<int, List<TestSettings>> Tests { get; set; } = new Dictionary<int, List<TestSettings>>();
        public int? GasTemperatureSensorId { get; set; }
        public int? LiquidTemperatureSensorId { get; set; }
        public bool IsEnableGasTemperatureSensor { get; set; }
        public bool IsEnableLiquidTemperatureSensor { get; set; }
        public int HideCamera { get; set; }
        public bool HidePtzPanel { get; set; }
        public bool ManualLocation { get; set; }
        public bool AutoRange { get; set; }
        public Dictionary<int, bool> Sensors { get; set; } = new Dictionary<int, bool>();
        public string ReportPath { get; set; }
        public string UrlForQr { get; set; } = string.Empty;
        public Pressure FuncionalityTestSensetive { get; set; } = new Pressure(1, PressureUnit.Bar);
        public int FuncionalityTestWindowSize { get; set; } = 10;
        public double FuncionalityTestValidSetPressurePercent { get; set; } = 3;
        public Pressure FuncionalTestMinimunChartSize { get; set; } = new Pressure(5, PressureUnit.Bar);
        public class TestSettings 
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string LocalName { get; set; }
            public TestType Type { get; set; }
            public int? Time { get; set; }
            public int? PressureSensorId { get; set; }
            public int? LeakageSensorId { get; set; }
            public int? StandartId { get; set; }
            public MediumType MediumType { get; set; }
            public Pressure SetPressure { get; set; } = new Pressure(0, PressureUnit.Bar);
            public bool UseValveSetPressure { get; set; }
            public bool UseRangeSetPressure { get; set; }
        }
    }

}
