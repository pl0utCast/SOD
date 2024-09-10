using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Messages.Reports;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.ViewModels.Controls;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace SOD.ViewModels.Reports
{
    public class ReportsViewModel : ReactiveObject
    {
        public ReportsViewModel(IReportService reportService, INavigationService navigationService, IDialogService dialogService, IBus bus, ILocalizationService localizationService)
        {
            ViewTitle = localizationService["Reports.Reports"];
            reportService.Report
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async r =>
                {
                    Report?.Dispose();
                    var pages = await reportService.ReportToImages(r);
                    Report = new ReportViewModel(r, pages);
                    foreach (var page in pages)
                    {
                        page.Dispose();
                    }
                    IsSaved = false;
                });

            Refresh = ReactiveCommand.CreateFromTask(async () =>
            {
                await reportService.RefreshAsync();
            });
            var canPrint = this.WhenAnyValue(x => x.Report, (ReportViewModel report) => report != null);
            Print = ReactiveCommand.CreateFromTask(async () =>
            {
                var printDialog = new PrintDialog();
                var print = printDialog.ShowDialog();
                if (print == true) await reportService.PrintAsync(Report.Report);
            }, canPrint);

            var canSave = this.WhenAnyValue(x => x.IsSaved, x => x.Report, (isSaved, report) => !isSaved 
                                            && report != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill);
                              
            Save = ReactiveCommand.Create(() =>
            {
                IsSaved = true;
                reportService.Save(Report.Report);
                bus.Publish(new App.Messages.Reports.ReportSaveMessage());
            }, canSave);

            Export = ReactiveCommand.Create(() =>
            {
                var vm = new Dialogs.ExportReportDialogViewModel(dialogService, reportService, Report.Report);
                dialogService.ShowDialog("ExportReportDialog", vm);
            }, canPrint);

            var canNew = this.WhenAnyValue(x => x.Report).Select(r => r != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill);
            New = ReactiveCommand.CreateFromTask(async () =>
            {
                if (reportService.CurrentReport != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill)
                {
                    var result = await dialogService.ShowDialogAsync("CreateNewReport", new YesNoDialogViewModel(dialogService));
                    if ((bool)result)
                    {
                        reportService.Save(reportService.CurrentReport);
                    }
                    bus.Publish(new CreateNewReportMessage());
                }
            }, canNew);

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
            }, canPrint);
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
            }, canPrint);


            SavedReports = ReactiveCommand.Create(() => navigationService.NavigateTo("SavedReports"));
        }
        [Reactive]
        public ReportViewModel Report { get; set; }
        [Reactive]
        public bool IsSaved { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> New { get; set; }
        public ReactiveCommand<Unit, Unit> Refresh { get; set; }
        public ReactiveCommand<Unit, Unit> Print { get; set; }
        public ReactiveCommand<Unit, Unit> Export { get; set; }
        public ReactiveCommand<Unit, Unit> SavedReports { get; set; }
        public ReactiveCommand<Unit, Unit> SaveReportTemplate { get; set; }
        public ReactiveCommand<Unit, Unit> ZoomIn { get; set; }
        public ReactiveCommand<Unit, Unit> ZoomOut { get; set; }
        public string ViewTitle { get; set; }
        [Reactive]
        public double ScaleFactor { get; set; } = 1;
        [Reactive]
        public bool IsZoomIn { get; set; } = true;
        [Reactive]
        public bool IsZoomOut { get; set; } = false;

        public double stepZoom { get; set; } = 0.3;
    }
}
