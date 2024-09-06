using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages.Commands
{
    public class Hold
    {
        public Hold(HoldStatus status, int time)
        {
            Status = status;
            Time = time;
        }
        public HoldStatus Status { get; set; }
        public int Time { get; set; }
    }

    public enum HoldStatus
    {
        Start,
        Stop
    }
}
