using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SOD.View.Reports
{
    internal class RestGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type tt, object parameter, CultureInfo ci)
        {
            return new GridLength((double)value - 1, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type tt, object parameter, CultureInfo ci)
        {
            throw new NotImplementedException();
        }
    }
}
