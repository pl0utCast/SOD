using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public interface IDeviceChannel
    {
        int Id { get; set; }
        object Value { get; set; }
        ChannelDataType DataType { get; set; }
    }
}
