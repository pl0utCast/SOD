using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    static class PressureUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            PressureUnit.Bar,
            PressureUnit.Megapascal,
            PressureUnit.Kilopascal,
            PressureUnit.KilogramForcePerSquareCentimeter,
            PressureUnit.PoundForcePerSquareInch,
            PressureUnit.Millibar,
            PressureUnit.MillimeterOfMercury,

        };
    }
}
