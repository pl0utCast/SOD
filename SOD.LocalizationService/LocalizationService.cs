using Newtonsoft.Json;
using NLog;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SOD.LocalizationService
{
    public class LocalizationService<TLocalization> : ILocalizationService, INotifyPropertyChanged where TLocalization : class, new()
    {
        private const string SETTINS_KEY = "LocalizationService";
        public event PropertyChangedEventHandler PropertyChanged;

        private ILogger logger = LogManager.GetCurrentClassLogger();
        private string localizationPath = "Localizations";
        private TLocalization localization = new TLocalization();
        private CultureInfo currentCulture;
        private readonly ISettingsService settingsService;

        public LocalizationService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), localizationPath)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), localizationPath));
            }

            Settings = settingsService.GetSettings(SETTINS_KEY, new Settings());
            if (Settings.CurrentLang != null)
            {
                CurrentCulture = new CultureInfo(Settings.CurrentLang);
            }
            else
            {
                CurrentCulture = new CultureInfo("ru-RU");
            }
                
            foreach (var lang in Settings.Langs)
            {
                SupportCulture.Add(new CultureInfo(lang));
            }
        }

        public void Load(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) return;
            if (cultureInfo.Name == "ru-RU") Localization = new TLocalization();
            var langFileName = cultureInfo.Name + ".json"; ;
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), localizationPath, langFileName))) return;
            using (var sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), localizationPath, langFileName)))
            {
                try
                {
                    Localization = JsonConvert.DeserializeObject<TLocalization>(sr.ReadToEnd());
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Ошибка загрузки локализации");
                }
            }
        }

        public void Save(CultureInfo cultureInfo)
        {
            //if (cultureInfo.Name == "ru-RU") return;
            var langFileName = cultureInfo.Name+".json";

            using (var sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), localizationPath, langFileName)))
            {
                try
                {
                    sw.Write(JsonConvert.SerializeObject(Localization, Formatting.Indented));
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Ошибка сохранения локализации");
                }
            }
        }

        public void SaveSettings()
        {
            Settings.Langs.Clear();
            foreach (var cultureInfo in SupportCulture)
            {
                Settings.Langs.Add(cultureInfo.Name);
            }
            
            if (CurrentCulture!=null)
            {
                Settings.CurrentLang = CurrentCulture.Name;
                Save(CurrentCulture);
            }
            settingsService.SaveSettings(SETTINS_KEY, Settings);
        }

        public string this[string name]
        {

            get
            {
                if (name == null) return string.Empty;
                var names = name.Split(".");
                PropertyInfo propInfo = null;
                object valObj = null;
                foreach (var propName in names)
                {
                    if (propInfo == null)
                    {
                        propInfo = Localization.GetType().GetProperty(propName);
                        valObj = propInfo?.GetValue(Localization);
                    }
                    else
                    {

                        propInfo = valObj.GetType().GetProperty(propName);
                        valObj = propInfo?.GetValue(valObj);
                    }

                }
                if (valObj != null && valObj is string) return (string)valObj;
                else return "!ERROR LOCAL!";
            }
            set
            {
                var names = name.Split(".");
                PropertyInfo propInfo = null;
                object valObj = null;
                
                foreach (var propName in names)
                {
                    if (propInfo == null)
                    {
                        propInfo = Localization.GetType().GetProperty(propName);
                        valObj = propInfo?.GetValue(Localization);
                    }
                    else
                    {
                        propInfo = valObj.GetType().GetProperty(propName);
                        if (propName != names.Last())
                            valObj = propInfo?.GetValue(valObj);
                    }
                }
                if (names.Count() == 1) valObj = Localization;
                propInfo?.SetValue(valObj, value);
            }
        }

        public TLocalization Localization
        {
            get
            {
                return localization;
            }
            private set
            {
                localization = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }


        public CultureInfo CurrentCulture
        {
            get=>currentCulture;
            set
            {
                currentCulture = value;
                Load(value);
                CultureInfo.CurrentUICulture = value;
            }
        }

        public List<CultureInfo> SupportCulture { get; private set; } = new List<CultureInfo>();
        public Settings Settings { get; set; }
    }
}
