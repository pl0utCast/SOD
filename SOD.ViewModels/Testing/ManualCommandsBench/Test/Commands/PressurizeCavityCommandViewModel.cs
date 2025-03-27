using SOD.App.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class PressurizeCavityCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public PressurizeCavityCommandViewModel(CommandConfig commandConfig)
        {

        }

        public string Cavity { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
