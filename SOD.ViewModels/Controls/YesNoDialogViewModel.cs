using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Controls
{
    public class YesNoDialogViewModel : ReactiveObject
    {
        public YesNoDialogViewModel(IDialogService dialogService)
        {
            Yes = ReactiveCommand.Create(() => dialogService.CloseAsync(true));
            No = ReactiveCommand.Create(() => dialogService.CloseAsync(false));
        }
        public ReactiveCommand<Unit, Unit> Yes { get; set; }
        public ReactiveCommand<Unit, Unit> No { get; set; }
    }
}
