using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace SOD.View.Converters
{
	public class ReverseBooleanToVisibilityConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var b = (bool)value;
			if (b)
			{
				return Visibility.Collapsed;
			}
			else
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return false;
		}
	}
}
