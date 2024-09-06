using SOD.App.Mediums;
using SOD.LocalizationService;

namespace SOD.View.Converters
{
    public class MediumTypeConverter : EnumConverterTemplate<MediumType>
    {
        public MediumTypeConverter() : base(mt=>
        {
            switch (mt)
            {
                case MediumType.Liquid:
                    return LocalizationExtension.LocaliztionService["MediumType.Water"];
                case MediumType.Gas:
                    return LocalizationExtension.LocaliztionService["MediumType.Air"];
                default: return null;
            }
        }) 
        
        {}
    }
}
