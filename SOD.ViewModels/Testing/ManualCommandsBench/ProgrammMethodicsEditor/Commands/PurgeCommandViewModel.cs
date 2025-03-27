
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class PurgeCommandViewModel : BaseCommandViewModel
    {
        public PurgeCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);
        }

        public override void Save()
        {

        }
    }
}
