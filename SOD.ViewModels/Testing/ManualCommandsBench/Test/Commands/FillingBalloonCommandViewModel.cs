using ReactiveUI;
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class FillingBalloonCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public FillingBalloonCommandViewModel(CommandConfig commandConfig)
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
