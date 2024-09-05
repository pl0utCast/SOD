using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device.PKTBAImpulseSensorBoard
{
    [DeviceSettings]
    public class Settings
    {
        public Settings()
        {

        }
        public string Name { get; set; }
        public string SerialPort { get; set; }
        public int UsedBaudRate { get; set; } = 9600;
        public bool UseDigitalSensors { get; set; } = false;
        public List<Channel> RequestChannels { get; set; } = new List<Channel>();

        public string SensorHint { get; set; }

        public class Channel 
        {
            public Channel(byte address, bool isEnable)
            {
                Address = address;
                IsEnable = isEnable;
            }
            public byte Address { get; set; }
            public bool IsEnable { get; set; }
        }
    }

}
