using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches
{
    public interface ITestBenchService
    {
        ITestBench GetTestBench();
        Settings Settings { get; }
        public void LoadSettings();
        public void SaveSettings();
    }
}
