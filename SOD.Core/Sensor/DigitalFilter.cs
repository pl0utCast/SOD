using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Sensor
{
    public class DigitalFilter
    {
        private double oldValue;

        public double Filtering(double val)
        {
            var filtered = (oldValue * Coeffecient + (1.0 - Coeffecient) * val);
            oldValue = filtered;
            return filtered;
        }

        public double Coeffecient { get; set; }
    }
}
