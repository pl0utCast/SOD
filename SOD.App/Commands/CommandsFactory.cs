using SOD.Core.Valves;
using SOD.App.Testing.Standarts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Commands
{
    public abstract class CommandsFactory
    {
        public abstract ICommand Create(CommandConfig commandConfig, Valve valve);
    }
}
