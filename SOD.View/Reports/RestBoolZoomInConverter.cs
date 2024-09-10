using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SOD.View.Reports
{
    internal class RestBoolZoomInConverter : IValueConverter
    {
        public object Convert(object value, Type tt, object parameter, CultureInfo ci)
        {
            return (bool)value;
        }

        public object ConvertBack(object value, Type tt, object parameter, CultureInfo ci)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
