using SOD.Core.Props;
using SOD.Core.Settings;
using UnitsNet;
using UnitsNet.Units;
using SOD.Core.Balloons;
using SOD.Core.Balloons.Properties;

namespace SOD.App.Benches.SODBench
{
	[ApplicationSettings]
	public class Settings : IBenchSettings
	{
		public TestSettings SelectedTestSettings { get; set; }
		public Balloon SelectedBalloon { get; set; }
		public PressureUnit PressureUnit { get; set; }
		public ForceUnit TenzoUnit { get; set; }
        public VolumeFlowUnit LeakageUnit { get; set; }
        public List<BalloonProperty> BalloonProperties { get; set; } = new List<BalloonProperty>();
		public List<Property> Parameters { get; set; } = new List<Property>();
		public Dictionary<int, List<TestSettings>> Tests { get; set; } = new Dictionary<int, List<TestSettings>>();
		public int? GasTemperatureSensorId { get; set; }
		public int? LiquidTemperatureSensorId { get; set; }
		public bool IsEnableGasTemperatureSensor { get; set; }
		public bool IsEnableLiquidTemperatureSensor { get; set; }
		public bool ManualLocation { get; set; }
		public bool AutoRange { get; set; }
		public Dictionary<int, bool> Sensors { get; set; } = new Dictionary<int, bool>();
        public Dictionary<int, bool> DevicesUnits { get; set; } = new Dictionary<int, bool>();
        public string ReportPath { get; set; }
		//public string UrlForQr { get; set; } = string.Empty;
		public class TestSettings
		{
            public string Name { get; set; }
            public int? Time { get; set; }
			public int? PressureSensorId { get; set; }
			public int WorkPressure { get; set; }
			public int Deformation { get; set; }
			public int? MaxDeformation { get; set; }
			public int TenzoSensorId { get; set; }
			public Pressure SetPressure { get; set; } = new Pressure(0, PressureUnit.Bar);
		}
	}
}
