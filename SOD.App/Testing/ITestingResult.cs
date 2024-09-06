using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing
{
    public interface ITestingResult
    {
        public bool IsFill { get; }
        public void Clear();
    }
}
