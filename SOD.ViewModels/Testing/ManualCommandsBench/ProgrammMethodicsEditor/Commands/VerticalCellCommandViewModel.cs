using SOD.App.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class VerticalCellCommandViewModel : BaseCommandViewModel 
    {
        public VerticalCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            if (commandConfig.Parameters.Count>0)
            {
                // параметер в виде давления
                if (commandConfig.Parameters[0].Key==typeof(double))
                {
                    Pressure = ((double)commandConfig.Parameters[0].Value).ToString("0.0");
                }
                //параметер в виде формулы
                else if (commandConfig.Parameters[0].Key == typeof(string))
                {
                    Pressure = (string)commandConfig.Parameters[0].Value;
                }
            }

            this.WhenActivated(dis =>
            {
                this.WhenAnyValue(x => x.Pressure)
                .Subscribe(p =>
                {
                    Notify(Validate());
                })
                .DisposeWith(dis);
            });
        }

        private bool Validate()
        {
            return true;
        }


        public override void Save()
        {
            CommandConfig.Parameters.Clear();
            if (double.TryParse(Pressure, out var pressure))
            {
                CommandConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(double), pressure));
            }
            else
            {
            }

        }
        [Reactive]
        public string Pressure { get; set; }
    }
}
