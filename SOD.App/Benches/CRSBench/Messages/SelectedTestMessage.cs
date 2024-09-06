using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Benches.CRSBench.Messages
{
    public class SelectedTestMessage
    {
        public SelectedTestMessage(TestType testType)
        {
            TestType = testType;
        }
        public TestType TestType { get; set; }
    }
}
