using SOD.App.Commands;
using System;

namespace SOD.App.Messages.Commands
{
    public class StopExecuteCommand
    {
        public StopExecuteCommand(bool isExecute)
        {
            IsExecute = isExecute;
        }
        public bool IsExecute { get; set; }
    }
}
