using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface ISensorService
    {
        ISensor GetSensor(int id);
        IEnumerable<ISensor> GetAllSensors();
    }
}
