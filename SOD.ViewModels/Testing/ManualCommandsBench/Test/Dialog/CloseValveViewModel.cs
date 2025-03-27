using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Dialog
{
    public class CloseValveViewModel : ReactiveObject
    {
        public CloseValveViewModel(IDialogService dialogService)
        {
            Apply = ReactiveCommand.Create(() => dialogService.CloseAsync(true));
        }
        public ReactiveCommand<Unit, Unit> Apply { get; set; }
    }
}
