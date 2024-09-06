using SOD.App.Testing.Programms;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages
{
    public class SelectProgrammMethodicsConfigMessage
    {
        public SelectProgrammMethodicsConfigMessage(ProgrammMethodicsConfig programmMethodicsConfig)
        {
            ProgrammMethodicsConfig = programmMethodicsConfig;
        }
        public ProgrammMethodicsConfig ProgrammMethodicsConfig { get; set; }
    }
}
