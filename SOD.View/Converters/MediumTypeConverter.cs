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

    public class MediumTypeConverter3Post : EnumConverterTemplate<MediumType3Post>
    {
        public MediumTypeConverter3Post() : base(mt =>
        {
            switch (mt)
            {
                case MediumType3Post.Liquid:
                    return LocalizationExtension.LocaliztionService["MediumType3Post.Water"];
                case MediumType3Post.LiquidHigh:
                    return LocalizationExtension.LocaliztionService["MediumType3Post.WaterHigh"];
                case MediumType3Post.Gas:
                    return LocalizationExtension.LocaliztionService["MediumType3Post.Air"];
                case MediumType3Post.GasHigh:
                    return LocalizationExtension.LocaliztionService["MediumType3Post.AirHigh"];
                default: return null;
            }
        })

        { }
    }
}
