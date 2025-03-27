using SOD.App.Commands;
using SOD.App.Mediums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class TestMediumCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public TestMediumCommandViewModel(CommandConfig commandConfig)
        {

        }


        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
