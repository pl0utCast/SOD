using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Sensor
{
    public interface ISensor
    {
        int Id { get; }
        public string Name { get; }
        public string SensorHint { get; }
        public string Accaury { get; }
        void SaveSettings();
        public void Connect();
        public void Disconnect();
    }
}
