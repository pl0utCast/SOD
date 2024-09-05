using SOD.Core.Props;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SOD.Core.Reports
{
    public class Report
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public BaseReportData ReportData { get; set; }
        public RawReport RawReport { get; set; } = new RawReport();
        public bool IsSave { get; set; }
        public string ReportTemplate { get; set; }
    }
}
