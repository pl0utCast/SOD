using SOD.LocalizationService;
using System.Globalization;
using System.Windows.Data;


namespace SOD.View.Converters
{
    public class TranslatePrefixConverter : IMultiValueConverter, IValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = LocalizationExtension.LocaliztionService["Prefixes." + value[0]]; // translation
            if (result == "!ERROR LOCAL!")
            {
                if (value[1] != null)
                    result = value[1] as string; // Name
                else
                    result = value[0] as string;  //prefix
            }
                
            return result;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = LocalizationExtension.LocaliztionService["Prefixes." + value];// translation

            if (result == "!ERROR LOCAL!")
                    result = value as string;//prefix

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
