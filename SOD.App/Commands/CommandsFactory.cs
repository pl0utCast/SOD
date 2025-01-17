using SOD.Core.Balloons;

namespace SOD.App.Commands
{
    public abstract class CommandsFactory
    {
        public abstract ICommand Create(CommandConfig commandConfig, Balloon valve);
    }
}
