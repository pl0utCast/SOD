using MemBus;
using SOD.App.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class VerticalCellCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public VerticalCellCommandViewModel(CommandConfig commandConfig, IBus bus)
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
