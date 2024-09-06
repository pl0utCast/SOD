using SOD.Core.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches
{
    public interface ITestBench
    {
        public BenchesType Type { get; }
        public IEnumerable<IPost> Posts { get; }
        public IBenchSettings Settings { get; }
        public Valve TestingValve { get; set; }
    }
}
