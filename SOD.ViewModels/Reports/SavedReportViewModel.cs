using SOD.Core.Reports;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Reports
{
	public class SavedReportViewModel : ReactiveObject
	{
		public SavedReportViewModel(Report report)
		{
			Report = report;
			BalloonType = GetParameter(report.Properties, "balloon_type");
			BalloonVolume = GetParameter(report.Properties, "balloon_volume");
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
		public string BalloonType { get; set; }
		public string BalloonVolume { get; set; }
		public string Date { get; set; }
		public int ReportNumber { get; set; }
		public Report Report { get; set; }

	}
}
