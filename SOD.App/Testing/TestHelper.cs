using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing
{
    public static class TestHelper
    { 
        public static string GetName(this TestType testType)
        {
            switch (testType)
            {
                case TestType.Strength:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.StrengthTest"];
                case TestType.Leakage:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.LeakageTest"];
                case TestType.Functional:
                    return LocalizationExtension.LocaliztionService["Settings.Standarts.FunctionalTest"];
                default:
                    return testType.ToString();
            }
        }
    }
}
