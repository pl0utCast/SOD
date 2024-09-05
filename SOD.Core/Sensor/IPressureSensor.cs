using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor
{
    public interface IPressureSensor : ISensor, IObservable<Pressure>
    {
        public Pressure Pressure { get; }
        public Pressure MaxValue { get; }
        public Pressure MinValue { get; }
    }
}
