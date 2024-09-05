using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    static class VolumeFlowUnits
    {
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            VolumeFlowUnit.CubicCentimeterPerMinute,
            VolumeFlowUnit.LiterPerMinute,
            VolumeFlowUnit.LiterPerSecond,
            VolumeFlowUnit.LiterPerHour,
            VolumeFlowUnit.CubicMeterPerHour,
            VolumeFlowUnit.CubicMeterPerMinute,
            VolumeFlowUnit.CubicMeterPerSecond
        };
    }
}
