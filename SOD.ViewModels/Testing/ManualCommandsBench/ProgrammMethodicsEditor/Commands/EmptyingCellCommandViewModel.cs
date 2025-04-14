using SOD.App.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public class EmptyingCellCommandViewModel : BaseCommandViewModel
    {
        public EmptyingCellCommandViewModel(CommandConfig commandConfig) : base(commandConfig)
        {
            Notify(true);
        }

        public override void Save()
        {
            
        }
    }
}
