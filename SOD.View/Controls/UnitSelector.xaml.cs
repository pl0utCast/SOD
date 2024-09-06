using SOD.Core.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Логика взаимодействия для UnitSelector.xaml
    /// </summary>
    public partial class UnitSelector : UserControl
    {
        public UnitSelector()
        {
            InitializeComponent();
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UnitSelector), new PropertyMetadata(null));





        public UnitTypeInfo Type
        {
            get { return (UnitTypeInfo)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(UnitTypeInfo), typeof(UnitSelector), new PropertyMetadata(null));





        public IEnumerable<UnitTypeInfo> Types
        {
            get { return (IEnumerable<UnitTypeInfo>)GetValue(TypesProperty); }
            set { SetValue(TypesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypesProperty =
            DependencyProperty.Register("Types", typeof(IEnumerable<UnitTypeInfo>), typeof(UnitSelector), new PropertyMetadata(null));
    }
}
