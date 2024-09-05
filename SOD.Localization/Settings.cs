using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization
{
    [ApplicationSettings]
    public class Settings
    {
        public List<string> Langs { get; set; } = new List<string>();
        public string CurrentLang { get; set; }
    }
}
