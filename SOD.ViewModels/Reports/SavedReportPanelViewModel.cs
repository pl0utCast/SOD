using SOD.Core.Infrastructure;
using SOD.Core.Reports;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SOD.ViewModels.Reports
{
    public class SavedReportPanelViewModel : ReactiveObject
    {
        public SavedReportPanelViewModel(IDialogService dialogService, INavigationService navigationService, IReportService reportService, Report report, List<Stream> pages) 
        {
            var loadReport = this.WhenAnyValue(x => x.Report, (ReportViewModel report) => pages.Count != 0);
            Report = new ReportViewModel(report, pages);
            GoBack = ReactiveCommand.Create(() =>
            {
                Report.Dispose();
                navigationService.GoBack();
            });
            Print = ReactiveCommand.CreateFromTask(async () =>
            {
                var printDialog = new PrintDialog();
                var print = printDialog.ShowDialog();
                if (print==true) await reportService.PrintAsync(report);
            }, loadReport);
            Export = ReactiveCommand.Create(() =>
            {
                var vm = new Dialogs.ExportReportDialogViewModel(dialogService, reportService, Report.Report);
                dialogService.ShowDialog("ExportReportDialog", vm);
            }, loadReport);

            ZoomIn = ReactiveCommand.CreateFromTask(async () =>
            {
                stepZoom = 0;
                while (stepZoom < 0.3 && ScaleFactor < 1.6)
                {
                    await Task.Delay(1);
                    ScaleFactor += 0.01;
                    stepZoom += 0.01;
                    IsZoomIn = false;
                    IsZoomOut = true;
                }
                if (ScaleFactor <= 1.5) IsZoomIn = true;
            }, loadReport);
            ZoomOut = ReactiveCommand.CreateFromTask(async () =>
            {
                stepZoom = 0;
                while (stepZoom < 0.3 && ScaleFactor > 1)
                {
                    await Task.Delay(1);
                    ScaleFactor -= 0.01;
                    stepZoom += 0.01;
                    IsZoomOut = false;
                    IsZoomIn = true;
                }
                if (ScaleFactor > 1) IsZoomOut = true;
            }, loadReport);
        }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Print { get; set; }
        public ReactiveCommand<Unit, Unit> Export { get; set; }
        public ReactiveCommand<Unit, Unit> ZoomIn { get; set; }
        public ReactiveCommand<Unit, Unit> ZoomOut { get; set; }
        public ReportViewModel Report { get; set; }

        [Reactive]
        public double ScaleFactor { get; set; } = 1;
        [Reactive]
        public bool IsZoomIn { get; set; } = true;
        [Reactive]
        public bool IsZoomOut { get; set; } = false;

        public double stepZoom { get; set; } = 0.3;
    }
}
