using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace SOD.View.Converters
{
    public class TestTypeConverter : MarkupExtension, IValueConverter
    {
        
        private Dictionary<string, TestType> names = new Dictionary<string, TestType>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type = (TestType)value;
            return names.SingleOrDefault(c => c.Value == Type).Key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return names[(string)value];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            names = Enum.GetValues(typeof(TestType)).Cast<TestType>().ToDictionary(tt => tt.GetName());
            Types = Enum.GetValues(typeof(TestType)).Cast<TestType>().Select(tt => tt.GetName()).ToList();
            return this;
        }

        public TestType Type { get; set; }
        public List<string> Types { get; set; } = new List<string>();
    }
}
