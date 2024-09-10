using SOD.Core.Device;
using System.Windows.Markup;

namespace SOD.View.MarkupExtensions
{
    public class ChannelDataTypeExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(typeof(ChannelDataType)).Cast<ChannelDataType>().ToList();
        }
    }
}
