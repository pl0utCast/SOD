using SOD.ViewModels.Testing.SODBench.Sensors;
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

namespace SOD.View.Testing.SODBench
{
    /// <summary>
    /// Логика взаимодействия для TemperatureSensorsViewModel.xaml
    /// </summary>
    public partial class TemperatureSensorsView : UserControl, IViewFor<TemperatureSensorsViewModel>
    {
        public TemperatureSensorsView()
        {
            InitializeComponent();
            this.WhenActivated(dis => { });
        }



        public TemperatureSensorsViewModel ViewModel
        {
            get { return (TemperatureSensorsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TemperatureSensorsViewModel), typeof(TemperatureSensorsView), new PropertyMetadata(null));


        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as TemperatureSensorsViewModel; }
    }
}
