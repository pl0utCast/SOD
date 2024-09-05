using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace SOD.Localization
{
    public class LocalizationExtension : Binding
    {
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            this.Mode = BindingMode.OneWay;
            this.Source = LocaliztionService;
        }

        public static ILocalizationService LocaliztionService { get; set; }
    }
}
