using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches
{
    public class BenchSensor
    {
        public BenchSensor(ISensor sensor)
        {
            Sensor = sensor;
        }
        public ISensor Sensor { get; }
    }
}
