using ReactiveUI;
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class PressureSetCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public PressureSetCommandViewModel(CommandConfig commandConfig)
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
