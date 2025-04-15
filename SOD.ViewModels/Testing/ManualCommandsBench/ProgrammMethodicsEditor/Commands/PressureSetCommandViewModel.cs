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
    public class PressureSetCommandViewModel : BaseCommandViewModel
    {
        public PressureSetCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);
            if (commandConfig.Type != CommandType.PressureSet) throw new ArgumentException("command is not support");

        }

        public override void Save()
        {
            CommandConfig.Parameters.Clear();
        }
    }
}
