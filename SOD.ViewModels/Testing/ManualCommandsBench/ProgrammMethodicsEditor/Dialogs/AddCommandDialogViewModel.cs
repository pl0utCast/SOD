using SOD.App.Commands;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Dialogs
{
    public class AddCommandDialogViewModel : ReactiveObject
    {
        public AddCommandDialogViewModel(IDialogService dialogService)
        {
            Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(null));
            Save = ReactiveCommand.Create(() => dialogService.CloseAsync(CommandsHelper.GetDefault(CommandCollectionType.Modbus3Post, SelectedCommand)));
        }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
        [Reactive]
        public CommandType SelectedCommand { get; set; }
    }
}
