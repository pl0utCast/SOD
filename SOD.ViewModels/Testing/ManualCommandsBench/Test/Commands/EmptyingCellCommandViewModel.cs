using MemBus;
using SOD.App.Commands;
using SOD.App.Messages;
using SOD.App.Messages.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class EmptyingCellCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public EmptyingCellCommandViewModel(CommandConfig commandConfig, IBus bus)
        {
            
        }
        [Reactive]
        public string Time { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
