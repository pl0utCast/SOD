using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    static class LengthUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            LengthUnit.Millimeter,
            LengthUnit.Centimeter,
            LengthUnit.Mil,
            LengthUnit.Inch
        };
    }
}
