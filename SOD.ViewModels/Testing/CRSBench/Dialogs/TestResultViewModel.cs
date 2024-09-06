using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Testing.CRSBench.Dialogs
{
    public class TestResultViewModel : ReactiveObject
    {
        public TestResultViewModel(IDialogService dialogService, IObservable<bool> canAdd)
        {
            Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(null));
        }
        
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
