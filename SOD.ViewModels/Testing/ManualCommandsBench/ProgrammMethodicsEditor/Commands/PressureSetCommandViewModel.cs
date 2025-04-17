using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class PressureSetCommandViewModel : BaseCommandViewModel
    {
        public PressureSetCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.PressureSet) throw new ArgumentException("command is not support");
            Notify(true);
        }

        public override void Save()
        {
            
        }
    }
}
