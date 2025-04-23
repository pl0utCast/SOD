using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class PressureReleaseCommandViewModel : BaseCommandViewModel
    {
        public PressureReleaseCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.PressureRelease) throw new ArgumentException("Command not support");
            Notify(true);
        }

        public override void Save()
        {

        }
    }
}
