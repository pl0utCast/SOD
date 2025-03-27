using ReactiveUI;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class PurgeCommandViewModel : IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
