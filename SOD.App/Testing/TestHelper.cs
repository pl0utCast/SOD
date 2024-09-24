using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing
{
    public static class TestHelper
    { 
        public static string GetName(this BalloonType balloonType)
        {
            switch (balloonType)
            {
                case BalloonType.KPG1:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG1"];
                case BalloonType.KPG2:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG2"];
                case BalloonType.KPG3:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG3"];
				case BalloonType.KPG4:
					return LocalizationExtension.LocaliztionService["Settings.Standarts.KPG4"];
				default:
                    return balloonType.ToString();
            }
        }
    }
}
