using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SOD.LocalizationService
{
    public interface ILocalizationService
    {
        public void SaveSettings();
        void Load(CultureInfo cultureInfo);
        void Save(CultureInfo cultureInfo);
        public string this[string name] { get; set; }
        public CultureInfo CurrentCulture { get; set; }
        public List<CultureInfo> SupportCulture { get; }
    }
}
