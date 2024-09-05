using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface ISettingsService
    {
        TSettings GetSettings<TSettings>(string settingsKey, TSettings defaultValue);
        void SaveSettings<TSettings>(string settingsKey, TSettings settings);
        void SaveSensorSettings<TSettings>(string settingsKey, TSettings settings);
    }
}
