using SOD.ViewModels.Settings.Bench.CRSBench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOD.View.Settings.Bench.CRSBench
{
	/// <summary>
	/// Логика взаимодействия для CrsBenchSettingsView.xaml
	/// </summary>
	public partial class CrsBenchSettingsView : UserControl
	{
		public CrsBenchSettingsView()
		{
			InitializeComponent();
			this.btnSelectReportPath.Click += BtnSelectReportPath_Click;
		}

		private void BtnSelectReportPath_Click(object sender, RoutedEventArgs e)
		{
			using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
			{
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Filter = "*.frx|*.frx";
				openFileDialog.Multiselect = false;
				if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var vm = DataContext as CRSBenchSettingsViewModel;
					vm.ReportPath = openFileDialog.FileName;
				}
			}
		}
	}
}
