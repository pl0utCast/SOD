using ReactiveUI;
using SOD.App.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class PressureReleaseCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public PressureReleaseCommandViewModel(CommandConfig commandConfig)
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
