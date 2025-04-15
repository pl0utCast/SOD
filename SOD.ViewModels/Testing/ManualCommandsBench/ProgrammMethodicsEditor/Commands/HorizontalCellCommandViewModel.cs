
using SOD.App.Commands;
using SOD.App.Mediums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class HorizontalCellCommandViewModel : BaseCommandViewModel
    {
        public HorizontalCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);

            if (commandConfig.Type == CommandType.HorizontalCell)
            {

            }
        }

        public override void Save()
        {
            CommandConfig.Parameters.Clear();
        }
    }
}
