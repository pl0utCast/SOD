using SOD.Core.Seals;
using SOD.LocalizationService;

namespace SOD.View.Converters
{
    public class TestMediumTypeConverter : EnumConverterTemplate<TestMediumType>
    {
        public TestMediumTypeConverter() : base(st =>
        {
            switch (st)
            {
                case TestMediumType.Water:
                    return LocalizationExtension.LocaliztionService["MediumType.Water"];
                case TestMediumType.Air:
                    return LocalizationExtension.LocaliztionService["MediumType.Air"];
                case TestMediumType.Gas:
                    return LocalizationExtension.LocaliztionService["MediumType.Gas"];
                default:
                    throw new NotSupportedException("Not support enum value " + st);
            }
        })
        { }
    }
}
