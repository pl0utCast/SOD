using SOD.App.Interfaces;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor;
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

namespace SOD.View.Testing.ManualCommandsBench.ProgrammMethodicsEditor
{
    /// <summary>
    /// Логика взаимодействия для EditProgrammMethodicsView.xaml
    /// </summary>
    public partial class EditProgrammMethodicsView : UserControl
    {
        public EditProgrammMethodicsView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           if( DataContext is EditProgrammMethodicsViewModel vm)
            {
                vm.SelectedCommand = (IBranch<ReactiveObject>)e.NewValue;
            }
        }
    }
}
