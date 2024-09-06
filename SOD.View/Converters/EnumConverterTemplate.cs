using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace SOD.View.Converters
{
    public class EnumConverterTemplate<T>: MarkupExtension, IValueConverter
    {
        private Func<T, string> _getName;
        
        private Dictionary<string, T> names = new Dictionary<string, T>();
        public EnumConverterTemplate(Func<T, string> getName)
        {
            _getName = getName;
        }
        
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type = (T)value;
            return names.SingleOrDefault(kv => kv.Value.Equals(Type)).Key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return names[(string)value];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            names = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(ct => _getName(ct));
            Types = Enum.GetValues(typeof(T)).Cast<T>().Select(ct => _getName(ct)).ToList();
            return this;
        }

        public List<string> Types { get; set; } = new List<string>();
        public T Type { get; set; }
    }
}
