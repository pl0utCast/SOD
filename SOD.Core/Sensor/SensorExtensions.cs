using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Sensor
{
    static class SensorExtensions
    {
        public static double CodeToValue(this ICodeBasedSensor sensor, double minValue, double maxValue, double minCode, double maxCode, double code)
        {
            if (maxCode == minCode) return 0;
            var k = (maxValue - minValue) / (maxCode - minCode);
            var b = maxValue - (k * maxCode);
            return (k * code + b);
        }
    }
}
