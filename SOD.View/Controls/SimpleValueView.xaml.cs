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

namespace SOD.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для SimpleValueView.xaml
    /// </summary>
    public partial class SimpleValueView : UserControl
    {
        public SimpleValueView()
        {
            InitializeComponent();
        }

        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }


        // Using a DependencyProperty as the backing store for FieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register("FieldName", typeof(string), typeof(SimpleValueView), new PropertyMetadata(""));
    }
}
