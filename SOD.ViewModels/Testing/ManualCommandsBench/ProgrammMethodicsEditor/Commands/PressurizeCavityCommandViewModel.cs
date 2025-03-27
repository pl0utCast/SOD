using SOD.App.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class PressurizeCavityCommandViewModel : BaseCommandViewModel
    {
        public PressurizeCavityCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);
            if (commandConfig.Type != CommandType.PressurizedCavity) throw new ArgumentException("command is not support");
            if(commandConfig.Parameters.Count>0)
            {
                
            }
            else
            {
                CavityA = true;
            }

            this.WhenActivated(dis =>
            {
                this.WhenAnyValue(x => x.CavityA)
                    .Where(c => !c && !CavityB && !CavityC)
                    .Subscribe(c => CavityA = true)
                    .DisposeWith(dis);

                this.WhenAnyValue(x => x.CavityB)
                    .Where(c => !c && !CavityA && !CavityC)
                    .Subscribe(c => CavityB = true)
                    .DisposeWith(dis);

                this.WhenAnyValue(x => x.CavityC)
                    .Where(c => !c && !CavityA && !CavityB)
                    .Subscribe(c => CavityC = true)
                    .DisposeWith(dis);
            });
        }
        [Reactive]
        public bool CavityA { get; set; }
        [Reactive]
        public bool CavityB { get; set; }
        [Reactive]
        public bool CavityC { get; set; }
        public override void Save()
        {
            CommandConfig.Parameters.Clear();
        }
    }
}
