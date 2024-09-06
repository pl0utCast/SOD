using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace SOD.ViewModels.Dialog
{
    public class IncorrectPasswordViewModel : ReactiveObject
    {
        public IncorrectPasswordViewModel(IDialogService dialogService)
        {
            Incorrect = ReactiveCommand.Create(() => dialogService.Close());
        }

        public ReactiveCommand<Unit, Unit> Incorrect { get; set; }
    }
}