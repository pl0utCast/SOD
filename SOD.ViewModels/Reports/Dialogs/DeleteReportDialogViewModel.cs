using SOD.ViewModels.Controls;
using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Reports.Dialogs
{
    public class DeleteReportDialogViewModel : YesNoDialogViewModel
    {
        public DeleteReportDialogViewModel(IDialogService dialogService) : base(dialogService)
        {

        }
    }
}
