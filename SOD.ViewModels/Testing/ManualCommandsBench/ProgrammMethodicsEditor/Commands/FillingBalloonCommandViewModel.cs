using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class FillingBalloonCommandViewModel : BaseCommandViewModel
    {
        public FillingBalloonCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.FillingBalloon) throw new ArgumentException("Command not support");
            Notify(true);
        }

        public override void Save()
        {

        }
    }
}
