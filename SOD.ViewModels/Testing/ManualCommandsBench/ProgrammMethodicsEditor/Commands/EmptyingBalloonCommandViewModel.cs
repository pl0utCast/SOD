using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Commands;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class EmptyingBalloonCommandViewModel : BaseCommandViewModel
    {
        public EmptyingBalloonCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Parameters.Count>0)
            {
                Time = (int)commandConfig.Parameters[0].Value;
                TimeInStandart = (bool)commandConfig.Parameters[1].Value;
            }

            this.WhenActivated(dis =>
            {
                this.WhenAnyValue(x => x.Time)
                .Subscribe(t =>
                {
                    if (t > 0 || TimeInStandart)
                        Notify(true);
                    else
                        Notify(false);
                }).DisposeWith(dis);
            });
        }

        public override void Save()
        {
            CommandConfig.Parameters.Clear();
            CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(int), Time));
            CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(bool), TimeInStandart));

        }
        [Reactive]
        public int Time { get; set; }
        [Reactive]
        public bool TimeInStandart { get; set; }
    }
}
