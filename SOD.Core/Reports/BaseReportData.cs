using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace SOD.Core.Reports
{
    public abstract class BaseReportData
    {
        public abstract List<KeyValuePair<string, object>> GetData();
        public abstract void Fill(params object[] parameters);
        public abstract bool IsFill { get; }
    }
}
