using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TensoSensor
{
	[SensorSettings]
	public class TensoSensorSettings
	{
		public int Id { get; set; }
		public int ChannelId { get; set; }
		public MassUnit MassUnit { get; set; } = MassUnit.Gram;
		public string Name { get; set; }
		public string Accaury { get; set; } = "F2";
		public double FilterCoef { get; set; } = 0;
		public string SensorHint { get; set; } = "";
	}
}
