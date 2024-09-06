using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Standarts
{
    public class StandartResult
    {
        public StandartResult(bool isValid, string info)
        {
            IsValid = isValid;
            Info = info;
        }
        public bool IsValid { get; }
        public string Info { get;  }
    }
}
