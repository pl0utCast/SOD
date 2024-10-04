using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Settings;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TensoSensor.CodeBased
{
	[SensorSettings]
	public class Settings
	{
		public Settings()
		{
			MinValue = UnitsNet.Mass.FromGrams(0);
			MaxValue = UnitsNet.Mass.FromGrams(100);
			Unit = MassUnit.Kilogram;
			Name = MaxValue.ToString();
		}
		public string Name { get; set; }
		public int ChannelId { get; set; }
		public int MaxCode { get; set; }
		public int MinCode { get; set; }
		[JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
		public Mass MinValue { get; set; }
		[JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
		public Mass MaxValue { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public MassUnit Unit { get; set; } = MassUnit.Kilogram;
		public string Accaury { get; set; } = "F2";
		public double FilterCoef { get; set; } = 0;
		public string SensorHint { get; set; } = "";
	}
}
