using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.Bench.CrsBench
{
    public class CrsBenchSettingsView
    {
        public string UseTempSensorWater { get; set; } = "Использовать датчик температуры (вода)";
        public string UseTempSensorAir { get; set; } = "Использовать датчик температуры (воздух)";
        public string ReportTemplate { get; set; } = "Шаблон протокола";
        public string Select { get; set; } = "Выбрать";
        public string UsedSensors { get; set; } = "Используемые датчики";
        public string Parameters { get; set; } = "Параметры";
        public string SaveSettings { get; set; } = "Настройки успешно сохранены!\nРекомендуется перезапустить программу";
        public string UseHideCamera { get; set; } = "Отображение интерфейса камеры";
        public string NoCamera { get; set; } = "Откл.";
        public string OneWindow { get; set; } = "В одном окне";
        public string TwoWindows { get; set; } = "В отдельном окне";
        public string ManualLocation { get; set; } = "Ручное расположение отчета";
        public string SensorsName { get; set; } = "Названия датчиков";
    }
}
