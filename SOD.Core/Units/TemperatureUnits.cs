using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    public static class TemperatureUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            TemperatureUnit.DegreeCelsius,
            TemperatureUnit.DegreeFahrenheit,
            TemperatureUnit.DegreeNewton,
            TemperatureUnit.Kelvin
        };
    }
}
