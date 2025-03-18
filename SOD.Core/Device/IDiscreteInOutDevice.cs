using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Device
{
    public interface IDiscreteInOutDevice
    {
        public void SetOutChannels(DiscreteChannel discreteChannel);
        public IEnumerable<DiscreteChannel> GetOutChannels();
        public class DiscreteChannel
        {
            public int Number { get; set; }
            public bool IsEnable { get; set; }
        }
    }

}
