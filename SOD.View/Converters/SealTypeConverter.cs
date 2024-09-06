using SOD.Core.Seals;
using SOD.LocalizationService;
using System;

namespace SOD.View.Converters
{
    public class SealTypeConverter : EnumConverterTemplate<SealType>
    {
        public SealTypeConverter() : base(st =>
        {
            switch (st)
            {
                case SealType.Metal:
                    return LocalizationExtension.LocaliztionService["SealType.MetallMetall"];
                case SealType.Polimer:
                    return LocalizationExtension.LocaliztionService["SealType.MetallPolimer"];
                default:
                    throw new NotSupportedException("Not support enum value " + st);
            }
        })
        { }
    }
}
