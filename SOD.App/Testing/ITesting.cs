using SOD.Core.Valves;
using SOD.App.Benches;
using SOD.App.Mediums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet.Units;

namespace SOD.App.Testing
{
    public interface ITesting
    {
        public bool IsRun { get; }
        public void StartCollectData();
        public void StartRegistration();
        public void StopRegistration();
        public void Start(ITestBench testBench);
        public void Stop();
        public void CalculateResult();
        public void FillReport(Bitmap chartImage = null);
        public TestType TestType { get; }
        public object[] GetTestParameters();
        public ITestingResult Result { get; }

    }
}
