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
		public TestBenchSettings TestBenchSettings { get; set; } = new TestBenchSettings();
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
    public class TestBenchSettings
    {
        public int OwnerDeviceId { get; set; }
        public int PressureClampSensorId { get; set; }
        public int PressureSensorId { get; set; }
        public int AmbientTemperatureSensorId { get; set; }
        public List<Property> Parameters { get; set; } = new List<Property>();
        public int PressureUnit { get; set; }
        public int LeakageUnit { get; set; }

        public double PressureVelocity { get; set; } = 11.0;
        public double HydraulicPressureCoef { get; set; } = 75.0;
        public double MaxAllowablePressure { get; set; } = 370.0;
        public double ThrottleActivationPercentage { get; set; } = 90.0;
        public double ThrottleDeactivationPercentage { get; set; } = 80.0;
        public double OverpressureAllowancePercentage { get; set; } = 1.0;
        public double NominalPressureValue { get; set; } = 600.0;
        public double Reserve1 { get; set; } = 100.0;
        public double Reserve2 { get; set; } = 100.0;
        public double VesselEmergencyLevel_5kg { get; set; } = 4500.0;
        public double VesselEmergencyLevel_10kg { get; set; } = 9000.0;
        public double VesselEmergencyLevel_30kg { get; set; } = 28000.0;
        public double KpPID { get; set; } = 0.0;
        public double KiPID { get; set; } = 3.0;
        public double KdPID { get; set; } = 0.0;
        public double dwePID { get; set; } = 0.0;
        public double tfPID { get; set; } = 0.3;
        public double MaxOutputConst { get; set; } = 100.0;
        public double ErrorZonePID { get; set; } = 0.0;
        public double MinOutputConst { get; set; } = 0.0;
        public double CyclePID { get; set; } = 2.0;
    }
}
