using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class VerticalCellCommandViewModel : BaseCommandViewModel
    {
        public VerticalCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.VerticalCell) throw new ArgumentException("command is not support");
            Notify(true);
        }

        public override void Save()
        {

        }
    }
}
