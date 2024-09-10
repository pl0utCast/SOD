using SOD.Core.Infrastructure;
using SOD.Core.Reports;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Reports.Dialogs
{
    public class ExportReportDialogViewModel: ReactiveObject, IActivatableViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> isExporting;
        public ExportReportDialogViewModel(IDialogService dialogService, IReportService reportService, Report report)
        {
            Cancel = ReactiveCommand.Create(() => dialogService.Close());
            var canExport = this.WhenAnyValue(x => x.Path, (string path) => path != null && Directory.Exists(path));
            Export = ReactiveCommand.CreateFromObservable(() => reportService.Export(report, Path), canExport);
            Export.IsExecuting.ToProperty(this, x => x.IsExporting, out isExporting);
            Export.ThrownExceptions.Subscribe(e => { });
        }
        public bool IsExporting => isExporting.Value;
        [Reactive]
        public string Path { get; set; }
        public ReactiveCommand<Unit, Unit> Export { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
