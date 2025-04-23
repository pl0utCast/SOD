using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class EmptyingCellCommandViewModel : BaseCommandViewModel
    {
        public EmptyingCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.EmptyingCell) throw new ArgumentException("Command not support");
            Notify(true);
        }

        public override void Save()
        {
            
        }
    }
}
