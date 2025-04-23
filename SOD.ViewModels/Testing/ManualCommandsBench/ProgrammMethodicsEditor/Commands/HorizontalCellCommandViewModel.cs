
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class HorizontalCellCommandViewModel : BaseCommandViewModel
    {
        public HorizontalCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.HorizontalCell) throw new ArgumentException("Command not support");
            Notify(true);
        }

        public override void Save()
        {
            
        }
    }
}
