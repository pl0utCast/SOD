using SOD.App.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages.Commands
{
    public class ExecuteTestCommand
    {
        public ExecuteTestCommand(CommandConfig commandConfig, bool isExecute)
        {
            CommandConfig = commandConfig;
            IsExecute = isExecute;
        }
        public bool IsExecute { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}
