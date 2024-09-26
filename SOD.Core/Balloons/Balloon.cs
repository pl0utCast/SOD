using SOD.Core.Valves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Core.Balloons.Properties;

namespace SOD.Core.Balloons
{
    public class Balloon
	{
		public string Name { get; set; }
		public BalloonType BalloonType { get; set; }
		public int BalloonVolume { get; set; }
		public int StandartId { get; set; }
		public string SerialNumber { get; set; }

		public List<BalloonProperty> Properties { get; private set; } = new List<BalloonProperty>();

	}
}
