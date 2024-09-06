using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.DeviceAndSensors
{
    public class Sensor
    {
        public string Name { get; set; } = "Название";
        public string CurrentCode { get; set; } = "Текущий код:";
        public string CurrentValue { get; set; } = "Текущее значение:";
        public string ImpulseCount { get; set; } = "Количество импульсов:";
        public string ChannelNumber { get; set; } = "Номер канала";
        public string MinValue { get; set; } = "Мин. значение";
        public string MaxValue { get; set; } = "Макс. значение";
        public string MinCode { get; set; } = "Мин. код";
        public string MaxCode { get; set; } = "Макс. код";
        public string SetCode { get; set; } = "Уст.";
        public string OutputNumber { get; set; } = "Номер выхода";
        public string Accaury { get; set; } = "Число знаков после запятой";
        public string FilterCoef { get; set; } = "Фильтрация";
        public string SensorHintAuto { get; set; } = "Автопереключение";
        public string SensorHintParallel { get; set; } = "Параллельные";
        public string UnitType { get; set; } = "Единицы измерения";
        public string Cancel { get; set; } = "ОТМЕНА";
        public string Save { get; set; } = "СОХРАНИТЬ";
        public string Reset { get; set; } = "СБРОСИТЬ";
        public string ImpulsePrice { get; set; } = "Цена импульса";
        public string ActiveSensor { get; set; } = "Активынй датчик id: ";
        public string SingleSensor { get; set; } = "Датчик id: ";
        public string SwitchDelay { get; set; } = "Задержка";
        public string LogicSwitchMIP { get; set; } = "Логика переключения";
        public string FiveSensors { get; set; } = "5 датчиков";
        public string FourSensorsIf { get; set; } = "4 датчика по условию";
        public string FourSensors { get; set; } = "4 датчика по одному";
    }
}
