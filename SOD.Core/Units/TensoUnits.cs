using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet.Units;

namespace SOD.Core.Units
{
	public static class TensoUnits
	{
		public static IReadOnlyList<Enum> Units => new List<Enum>()
		{
			MassUnit.Kilogram,
			MassUnit.Gram,
			MassUnit.Tonne
		};
	}
}
