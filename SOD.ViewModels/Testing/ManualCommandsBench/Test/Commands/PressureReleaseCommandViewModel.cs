using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class PressureReleaseCommandViewModel : IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
