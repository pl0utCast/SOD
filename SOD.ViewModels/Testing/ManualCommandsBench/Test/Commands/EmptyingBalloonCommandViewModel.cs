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
            this.WhenActivated(dis =>
            {
                bus.Subscribe<ValveFilling>(m =>
                {
                    Time = m.Time;
                })
                .DisposeWith(dis);

                bus.Subscribe<ValveFill>(m =>
                {
                    
                })
                .DisposeWith(dis);
                
                
            });

            Application.Current.Dispatcher.Invoke(async () =>
            {
                var result = (bool)await dialogService.ShowDialogAsync("OpenValveManual", new Dialog.OpenValveManual(dialogService));
                if (result)
                {
                    bus.Publish(new OpenManualValve());
                }
            });
        }
        public int Time { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
