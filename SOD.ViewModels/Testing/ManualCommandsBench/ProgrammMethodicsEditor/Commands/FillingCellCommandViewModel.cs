using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class FillingCellCommandViewModel : BaseCommandViewModel
    {
        public FillingCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.FillingCell) throw new ArgumentException("command is not support");
            Notify(true);
        }

        public override void Save()
        {
            
        }
    }
}
