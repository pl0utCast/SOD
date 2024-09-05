using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SOD.Core.Reports;
using System.IO;
using System.Threading.Tasks;
using System.Reactive;

namespace SOD.Core.Infrastructure
{
    public interface IReportService
    {
        public Task<Report> CreateReportAsync(BaseReportData baseReportData, string reportTemplate);
        public void Save(Report report);
        public IEnumerable<Report> GetReports();
        public Report GetReport(int id);
        public IObservable<Report> Report { get; }
        public Report CurrentReport { get; }
        public Task RefreshAsync();
        public Task PrintAsync(Report report);
        public Task<List<Stream>> ReportToImages(Report report);
        public void Remove(int id);
        public int GetReportNumber();
        public IObservable<Unit> Export(Report report, string path);
        public void SaveReportTemplate();
        public Task<int> EditReportTemplate(string reportPath);
    }
}
