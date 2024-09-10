using SOD.Core.Reports;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SOD.ViewModels.Reports
{
    public class ReportViewModel : ReactiveObject, IDisposable
    {
        public ReportViewModel(Report report, List<Stream> pages)
        {
            foreach (var image in pages)
            {
                Pages.Add(new ReportPageViewModel(image));
            }
            Report = report;
        }
        public List<ReportPageViewModel> Pages { get; set; } = new List<ReportPageViewModel>();

        public void Dispose()
        {
            foreach (var page in Pages)
            {
                page.Page.StreamSource.Dispose();
            }
        }

        public Report Report { get; set; }
    }
}
