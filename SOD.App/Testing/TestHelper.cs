using SOD.Core.Balloons;
using SOD.LocalizationService;

namespace SOD.App.Testing
{
    public static class TestHelper
    { 
        public static string GetName(this BalloonTypes balloonType)
        {
            switch (balloonType)
            {
                case BalloonTypes.KPG1:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG1"];
                case BalloonTypes.KPG2:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG2"];
                case BalloonTypes.KPG3:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG3"];
				case BalloonTypes.KPG4:
					return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG4"];
				default:
                    return balloonType.ToString();
            }
        }
    }
}
