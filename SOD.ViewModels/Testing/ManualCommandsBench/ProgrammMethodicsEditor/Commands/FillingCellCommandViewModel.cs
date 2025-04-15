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
    public class FillingCellCommandViewModel : BaseCommandViewModel
    {
        public FillingCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);
            if (commandConfig.Type != CommandType.FillingCell) throw new ArgumentException("command is not support");
        }

        public override void Save()
        {
            CommandConfig.Parameters.Clear();
        }
    }
}
