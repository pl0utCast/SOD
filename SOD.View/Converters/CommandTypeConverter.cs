using SOD.App.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace SOD.View.Converters
{
    public class CommandTypeConverter : MarkupExtension, IValueConverter
    {
        public List<string> Types { get; set; } = new List<string>();
        private Dictionary<string, CommandType> names = new Dictionary<string, CommandType>();
        public CommandType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type = (CommandType)value;
            return names.SingleOrDefault(c => c.Value == Type).Key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return names[(string)value];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            names = Enum.GetValues(typeof(CommandType)).Cast<CommandType>().ToDictionary(ct => ct.GetCommandName());
            Types = Enum.GetValues(typeof(CommandType)).Cast<CommandType>().Select(ct=>ct.GetCommandName()).ToList();
            return this;
        }
    }
}
