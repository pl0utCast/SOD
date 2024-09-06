using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages.Commands
{
    public class RegistrationMessage
    {
        public RegistrationMessage(RegistartionStatus registartionStatus, int time)
        {
            Status = registartionStatus;
            Time = time;
        }
        public RegistartionStatus Status { get; set; }
        public int Time { get; set; }
    }

    public enum RegistartionStatus
    {
        Start,
        End
    }
}
