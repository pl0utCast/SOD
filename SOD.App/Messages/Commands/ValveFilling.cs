using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages
{
    public class ValveFilling
    {
        public ValveFilling(bool isManual, int time)
        {
            IsManual = isManual;
            Time = time;
        }
        public bool IsManual { get; set; }
        public int Time { get; set; }
    }
}
