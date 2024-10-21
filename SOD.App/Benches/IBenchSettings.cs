using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet.Units;

namespace SOD.App.Benches
{
    public interface IBenchSettings
    {
        public PressureUnit PressureUnit { get; set; }
        //public VolumeFlowUnit LeakageUnit { get; set; }
    }
}
