using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public interface IReadWriteDevice
    {
        public T Read<T>(int channel);
        public void Write<T>(int channel, T data);
    }
}
