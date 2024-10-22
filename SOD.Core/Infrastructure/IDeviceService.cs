using SOD.Core.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface IDeviceService
    {
        IEnumerable<IDevice> GetAllDevice();
        IDevice GetDevice(int Id);
    }
}
