using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class EmptyingBalloonCommandViewModel : BaseCommandViewModel
    {
        public EmptyingBalloonCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.EmptyingBalloon) throw new ArgumentException("Command not support");
            Notify(true);
        }

        public override void Save()
        {

        }
    }
}
