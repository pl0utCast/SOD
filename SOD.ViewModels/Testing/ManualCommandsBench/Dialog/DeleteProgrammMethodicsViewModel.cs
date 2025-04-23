using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Dialog
{
    public class DeleteProgrammMethodicsViewModel
    {
        public DeleteProgrammMethodicsViewModel(IDialogService dialogService)
        {
            Delete = ReactiveCommand.Create(() => dialogService.CloseAsync(true));
            Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(false));
        }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
