using SOD.ViewModels.Testing.ManualCommandsBench;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SOD.View.Testing.ManualCommandsBench
{
    /// <summary>
    /// Логика взаимодействия для SelectProgrammMethodicsView.xaml
    /// </summary>
    public partial class SelectProgrammMethodicsView : UserControl, IViewFor<SelectProgrammMethodicsViewModel>
    {
        public SelectProgrammMethodicsView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as SelectProgrammMethodicsViewModel;
        }


        public SelectProgrammMethodicsViewModel ViewModel { get; set; }
        object IViewFor.ViewModel 
        { 
            get
            {
                return ViewModel;
            }
            set
            {
                ViewModel = (SelectProgrammMethodicsViewModel)value;
            }
        }
    }
}
