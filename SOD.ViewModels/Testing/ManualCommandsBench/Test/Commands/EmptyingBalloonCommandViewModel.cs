using MemBus;
using SOD.App.Messages;
using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class EmptyingBalloonCommandViewModel : ReactiveObject, IActivatableViewModel
    {
        public EmptyingBalloonCommandViewModel(IBus bus, IDialogService dialogService)
        {
            
        }
        public int Time { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
