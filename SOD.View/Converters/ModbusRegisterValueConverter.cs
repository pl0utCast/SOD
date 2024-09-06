using SOD.Core.Device;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace SOD.View.Converters
{
    public class ModbusRegisterValueConverter : MarkupExtension, IMultiValueConverter
    {
        public ChannelDataType DataType { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DataType = (ChannelDataType)values[1];
            if (DataType==ChannelDataType.BOOL)
            {
                return (bool)values[0];
            }
            return values[0]?.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            switch (DataType)
            {
                case ChannelDataType.INT16:
                    if (Int16.TryParse((string)value, out var int16result))
                    {
                        return new object[] { int16result };
                    }
                    else return null;    
                case ChannelDataType.UINT16:
                    if (UInt16.TryParse((string)value, out var uint16result))
                    {
                        return new object[] { uint16result };
                    }
                    else return null;
                case ChannelDataType.INT:
                    if (int.TryParse((string)value, out var intresult))
                    {
                        return new object[] { intresult };
                    }
                    else return null;
                case ChannelDataType.UINT:
                    if (uint.TryParse((string)value, out var uintresult))
                    {
                        return new object[] { uintresult };
                    }
                    else return null;
                case ChannelDataType.BOOL:
                    return new object[] { (bool)value };
                case ChannelDataType.STRING:
                    return new object[] { value };
                case ChannelDataType.DOUBLE:
                    if (double.TryParse((string)value, out var doubleresult))
                    {
                        return new object[] { doubleresult };
                    }
                    else return null;
                case ChannelDataType.FLOAT:
                    if (double.TryParse((string)value, out var floatresult))
                    {
                        return new object[] { floatresult };
                    }
                    else return null;
            }
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
