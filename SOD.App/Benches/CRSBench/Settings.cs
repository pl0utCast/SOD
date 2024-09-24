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
using ReactiveUI.Fody.Helpers;

namespace SOD.App.Benches.CRSBench
{
    [ApplicationSettings]
    public class Settings : IBenchSettings
    {
        public TestSettings SelectedTestSettings { get; set; }
        public int BalloonValue { get; set; }
        public string BalloonSN {  get; set; }
		public PressureUnit PressureUnit { get; set; }
        public List<Property> Parameters { get; set; } = new List<Property>();
        public Dictionary<int, List<TestSettings>> Tests { get; set; } = new Dictionary<int, List<TestSettings>>();
        public int? GasTemperatureSensorId { get; set; }
        public int? LiquidTemperatureSensorId { get; set; }
        public bool IsEnableGasTemperatureSensor { get; set; }
        public bool IsEnableLiquidTemperatureSensor { get; set; }
        public bool ManualLocation { get; set; }
        public bool AutoRange { get; set; }
        public Dictionary<int, bool> Sensors { get; set; } = new Dictionary<int, bool>();
        public string ReportPath { get; set; }
        public string UrlForQr { get; set; } = string.Empty;
        public class TestSettings 
        {
            //public string Name { get; set; }
            public BalloonType BalloonType { get; set; }
            public int? Time { get; set; }
            public int? PressureSensorId { get; set; }
            public int? StandartId { get; set; }
			public int WorkPressure { get; set; }
			public int Deformation { get; set; }
			public int? MaxDeformation { get; set; }
			public bool IsModeAuto { get; set; }
            public Pressure SetPressure { get; set; } = new Pressure(0, PressureUnit.Bar);
		}
    }
}
