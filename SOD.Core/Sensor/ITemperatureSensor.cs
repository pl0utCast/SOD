using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor
{
    public interface ITemperatureSensor : ISensor
    {
        Temperature Temperature { get; }
    }
}
