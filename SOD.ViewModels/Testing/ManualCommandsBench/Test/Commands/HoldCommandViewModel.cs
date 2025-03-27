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
    public class HoldCommandViewModel: ReactiveObject, IActivatableViewModel
    {
        private IDisposable timerDisposable;
        public HoldCommandViewModel(CommandConfig commandConfig, IBus bus)
        {
            this.WhenActivated(dis =>
            {
                bus.Subscribe<Hold>(m =>
                {
                    if (m.Status==HoldStatus.Start)
                    {
                        var counter =  TimeSpan.FromSeconds(m.Time);
                        timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                        .Subscribe(_ =>
                        {
                            if (counter.TotalSeconds >= 0)
                            {
                                Time = counter.ToString("mm\\:ss");
                                counter -= TimeSpan.FromSeconds(1);
                            }
                        })
                        .DisposeWith(dis);
                    }
                })
                .DisposeWith(dis);

                bus.Subscribe<ProgrammMethodicsStatus>(c =>
                {
                    if (c.Status != ProgrammStatus.Run)
                    {
                        timerDisposable.Dispose();
                        Time = string.Empty;
                    }
                })
               .DisposeWith(dis);
            });
        }
        [Reactive]
        public string Time { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
