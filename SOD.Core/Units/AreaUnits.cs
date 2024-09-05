using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    static class AreaUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            AreaUnit.SquareMillimeter,
            AreaUnit.SquareCentimeter,
            AreaUnit.SquareInch
        };
    }
}
