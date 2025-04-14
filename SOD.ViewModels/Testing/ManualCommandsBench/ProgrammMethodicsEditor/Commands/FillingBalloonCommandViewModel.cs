using SOD.App.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class FillingBalloonCommandViewModel : BaseCommandViewModel
    {
        public FillingBalloonCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Type != CommandType.FillingBalloon) throw new ArgumentException("Command not support");
            this.WhenActivated(dis =>
            {
                this.WhenAnyValue(x => x.IsManual)
                    .Where(m => m == true)
                    .Subscribe(m => IsValveTime = false)
                    .DisposeWith(dis);

                this.WhenAnyValue(x => x.IsValveTime)
                    .Where(vt => vt == true)
                    .Subscribe(vt =>
                    {
                        IsManual = false;
                        Notify(true);
                    })
                    .DisposeWith(dis);
                this.WhenAnyValue(x => x.Time, x => x.IsManual, (time, isManual) => time > 0 && isManual)
                    .Subscribe(t =>
                    {
                        Notify(t);
                    })
                    .DisposeWith(dis);
            });

            if (commandConfig.Parameters.Count > 0)
            {
                var isManual = (bool)commandConfig.Parameters[0].Value;
                if (isManual)
                {
                    IsManual = true;
                    Time = (int)commandConfig.Parameters[1].Value;
                }
                else
                {
                    IsValveTime = true;
                }
            }
            else
            {
                Notify(false);
            }
        }
        [Reactive]
        public bool IsManual { get; set; }
        [Reactive]
        public bool IsValveTime { get; set; }
        [Reactive]
        public int Time { get; set; }

        public override void Save()
        {
            CommandConfig.Parameters.Clear();
            if (IsManual)
            {
                CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(bool), true));
                CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(int), Time));
            }
            else if (IsValveTime)
            {
                CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(bool), false));
                CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(string), "filling_time"));
            }
        }
    }
}
