using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public interface IDevice
    {
        string Name { get; }
        int Id { get; }
        public string SensorHint { get; }
        void Connenct();
        void Disconnect();
        IObservable<DeviceStatus> Status { get; }
        DeviceStatus GetStatus();
    }
}
