using SOD.ViewModels.Settings.Bench.SODBench;
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

namespace SOD.View.Settings.Bench.SODBench
{
	/// <summary>
	/// Логика взаимодействия для SodBenchSettingsView.xaml
	/// </summary>
	public partial class SodBenchSettingsView : UserControl
	{
		public SodBenchSettingsView()
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
					var vm = DataContext as SODBenchSettingsViewModel;
					vm.ReportPath = openFileDialog.FileName;
				}
			}
		}
	}
}
