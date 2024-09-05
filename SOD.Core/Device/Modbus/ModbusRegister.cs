using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device.Modbus
{
    public class ModbusRegister : IDeviceChannel
    {
        public int RegisterId { get; set; }
        [JsonIgnore]
        public object Value { get; set; }
        public int Id 
        {
            get
            {
                return RegisterId;
            }
            set
            {
                RegisterId = value;
            }
        }
        public ChannelDataType DataType { get; set; }
        public int ByteCount { get; set; }
        public string Description { get; set; }
    }
}
