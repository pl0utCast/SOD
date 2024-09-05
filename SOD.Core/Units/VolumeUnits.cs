using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    public static class VolumeUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            
            VolumeUnit.CubicMillimeter,
            VolumeUnit.CubicCentimeter,
            VolumeUnit.CubicMeter,
            VolumeUnit.CubicInch
        };
    }
}
