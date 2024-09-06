using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches
{
    public interface IPost
    {
        public int Id { get; }
        public bool IsEnable { get; set; }
        public IEnumerable<BenchSensor> Sensors { get; }
        public string SerialNumber { get; }
        public PostStatus Status { get; set; }
    }
}
