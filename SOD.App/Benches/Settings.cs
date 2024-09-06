using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches
{
    [ApplicationSettings]
    public class Settings
    {
        public BenchesType BenchType { get; set; }
    }
}
