using ReactiveUI;
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class HorizontalCellCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public HorizontalCellCommandViewModel(CommandConfig commandConfig)
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
