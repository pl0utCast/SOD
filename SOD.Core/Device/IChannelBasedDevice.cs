using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public interface IChannelBasedDevice
    {
        IObservable<IDeviceChannel> DataComplite { get; }
    }
}
