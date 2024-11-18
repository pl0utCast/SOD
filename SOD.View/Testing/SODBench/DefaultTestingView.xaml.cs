using SOD.ViewModels.Testing.SODBench;
using ReactiveUI;
using SciChart.Charting;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOD.View.Testing.SODBench
{
    /// <summary>
    /// Логика взаимодействия для DefaultTestingView.xaml
    /// </summary>
    public partial class DefaultTestingView : UserControl
    {
        public DefaultTestingView()
        {
            InitializeComponent();       
        }

        public DefaultTestingViewModel ViewModel { get; set; }
    }
}
