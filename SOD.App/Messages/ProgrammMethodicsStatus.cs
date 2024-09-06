using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages
{
    public class ProgrammMethodicsStatus
    {
        public ProgrammMethodicsStatus(ProgrammStatus status)
        {
            Status = status;
        }
        public ProgrammStatus Status { get; set; }
    }

    public enum ProgrammStatus
    {
        Run,
        Stop,
        Chart,
        Error
    }
}
