using SOD.ViewModels.Testing.CRSBench.Sensors;
using ReactiveUI;
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

namespace SOD.View.Testing.CRSBench
{
    /// <summary>
    /// Логика взаимодействия для SensorView.xaml
    /// </summary>
    public partial class SensorView : UserControl, IViewFor<SensorViewModel>
    {
        public SensorView()
        {
            InitializeComponent();
            this.WhenActivated(dis => { });
        }

        


        public SensorViewModel ViewModel
        {
            get { return (SensorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SensorViewModel), typeof(SensorView), new PropertyMetadata(null));


        object IViewFor.ViewModel { get => ViewModel; set => ViewModel=(SensorViewModel)value; }
        public string SensorName
        {
            get { return (string)GetValue(SensorNameProperty); }
            set { SetValue(SensorNameProperty, value); }
        }


        // Using a DependencyProperty as the backing store for SensorName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SensorNameProperty =
            DependencyProperty.Register("SensorName", typeof(string), typeof(SensorView), new PropertyMetadata("Название"));


    }
}
