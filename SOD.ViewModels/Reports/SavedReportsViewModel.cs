using DynamicData;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;

namespace SOD.ViewModels.Reports
{
    public class SavedReportsViewModel : ReactiveObject, IActivatableViewModel
    {
        public SavedReportsViewModel(IReportService reportService, INavigationService navigationService, IDialogService dialogService)
        {
            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
            Reports.AddRange(reportService.GetReports().Select(r => new SavedReportViewModel(r)));
            
            this.WhenActivated(dis =>
            {
                this.WhenAnyValue(x => x.SelectAll)
                    .Subscribe(s =>
                    {
                        foreach (var report in Reports)
                        {
                            report.IsSelected = s;
                        }
                    })
                    .DisposeWith(dis);
            });

            var canShow = this.WhenAnyValue(x => x.SelectedReport, (SavedReportViewModel sr) => sr != null);

            Show = ReactiveCommand.CreateFromTask(async () =>
            {
                var report = reportService.GetReport(SelectedReport.Report.Id);
                var pages = await reportService.ReportToImages(report);
                var vm = new SavedReportPanelViewModel(dialogService, navigationService, reportService, report, pages);
                foreach (var page in pages)
                {
                    page.Dispose();
                }
                navigationService.NavigateTo("ShowSavedReport", vm);
            }, canShow);

            Delete = ReactiveCommand.CreateFromTask(async () =>
            {
                var deletedReports = Reports.Where(r => r.IsSelected).ToList();
                if (deletedReports.Count>0)
                {
                    var result = (bool)await dialogService.ShowDialogAsync("DeleteReportDialog", new Dialogs.DeleteReportDialogViewModel(dialogService));
                    if (result)
                    {
                        foreach (var report in deletedReports)
                        {
                            Reports.Remove(report);
                            reportService.Remove(report.Report.Id);
                        }
                    }
                }
                
            });
        }
        [Reactive]
        public bool SelectAll { get; set; }
        [Reactive]
        public SavedReportViewModel SelectedReport { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Show { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ObservableCollection<SavedReportViewModel> Reports { get; set; } = new ObservableCollection<SavedReportViewModel>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
