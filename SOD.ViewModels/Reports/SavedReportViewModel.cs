using SOD.Core.Reports;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Reports
{
    public class SavedReportViewModel: ReactiveObject
    {
        public SavedReportViewModel(Report report)
        {
            Report = report;
            ValveName = GetParameter(report.Properties, "valve_name");
            ValveType = GetParameter(report.Properties, "valve_type");
            DN = GetParameter(report.Properties, "DN");
            PN = GetParameter(report.Properties, "PN");
            Date = report.Date.ToString();
            ReportNumber = report.Number;
        }

        private string GetParameter(Dictionary<string, string> dic, string key)
        {
            if (dic.TryGetValue(key, out var result))
            {
                return result;
            }
            return null;
        }
        [Reactive]
        public bool IsSelected { get; set; }
        public string ValveName { get; set; }
        public string ValveType { get; set; }
        public string DN { get; set; }
        public string PN { get; set; }
        public string Sealing { get; set; }
        public string Date { get; set; }
        public int ReportNumber { get; set; }
        public Report Report { get; set; }

    }
}
