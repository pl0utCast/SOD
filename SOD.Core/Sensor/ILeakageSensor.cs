using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor
{
    public interface ILeakageSensor : ISensor , IObservable<VolumeFlow>
    {
        public void Reset();
        VolumeFlow Flow { get; }
        Volume Volume { get; }
        public VolumeFlow MaxValue { get; }
    }
}
