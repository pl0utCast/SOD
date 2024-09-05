using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public class DeviceChannel : IDeviceChannel
    {
        public int Id { get; set; }
        public object Value { get; set; }
        public ChannelDataType DataType { get; set; }
    }
}
