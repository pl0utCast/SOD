using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.Bench
{
    public class TestBenchSettingsView
    {
        public string BenchType { get; set; } = "Тип стенда";
        public string CurrentLang { get; set; } = "Выбранный язык";
        public string Lang { get; set; } = "Язык";
        public string Add { get; set; } = "ДОБАВИТЬ";
        public string Cancel { get; set; } = "ОТМЕНА";
        public string UseVirtualKeyboard { get; set; } = "Использовать виртуальную клавиатуру";
        public string UseAutoRange { get; set; } = "Использовать автомасштабирование";
    }
}
