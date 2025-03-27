using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Testing.ManualCommandsBench
{
    public class ManualCommandsBench
    {
        public string ModbusOffline { get; set; } = "Модбас не в сети";

        public string Parameters { get; set; } = "Параметры";
        public string Add { get; set; } = "Добавить";
        public string Delete { get; set; } = "Удалить";
        public string ChoiceOfTestMethod { get; set; } = "Выбор методики испытаний";
        public string ValveSelect { get; set; } = "Выбор арматуры";
        public string Cancel { get; set; } = "ОТМЕНА";
        public string Apply { get; set; } = "ПРИМЕНИТЬ";
        public string Units { get; set; } = "Единицы измерения";
        public string Pressure { get; set; } = "Давление";
        public string Command { get; set; } = "КОМАНДА";
        public string Test { get; set; } = "ИСПЫТАНИЕ";
        public string Name { get; set; } = "Название";
        public string ValveType { get; set; } = "Тип арматуры";
        public string EditingTestMethod { get; set; } = "Редактирование методики испытания";
        public string PostsCount { get; set; } = "Количество постов";
        public string ControlDevice { get; set; } = "Управляющее устройство";
        public string SensorScript { get; set; } = "Скрипт выбора датчиков";

        public string TestMedium { get; set; } = "Среда испытания";
        public string Filling { get; set; } = "Заполнение";
        public string PressurizedCavity { get; set; } = "Полость подачи давления";
        public string LeakControlCavity { get; set; } = "Полость контроля утечки";
        public string SetPressure { get; set; } = "Установить давление";
        public string Hold { get; set; } = "Выдержка";
        public string Registration { get; set; } = "Регистрация";
        public string PressureRelease { get; set; } = "Сброс давления";
        public string FillingTimeValve { get; set; } = "Время заполнения из параметров арматуры";
        public string FillingTimeManual { get; set; } = "Ручной ввод времени заполнения";
        public string FillTimeSeconds { get; set; } = "Время заполнения (секунды)";
        public string UseFromStandard { get; set; } = "Использовать из стандарта";
        public string HoldTimeSeconds { get; set; } = "Время выдержки (секунды)";
        public string RegistrationTimeSeconds { get; set; } = "Время регистрации (секунды)";
        public string PressureOrFormula { get; set; } = "Давление(бар) или формула";
        public string SelectedCavity { get; set; } = "Выбранная полость";
        public string FeedСavity { get; set; } = "Полость подачи";
        public string PressureSetting { get; set; } = "Установка давления";
        public string PressureBar { get; set; } = "Давление (бар)";
        public string Example { get; set; } = "Пример: PN*1.1";
        public string Purge { get; set; } = "Продувка";

        public string NoStandart { get; set; } = "Без стандарта";
        public string Standart { get; set; } = "Стандарт";
        public string ControlMethod { get; set; } = "Метод контроля";
        public string Confirm { get; set; } = "ПОДТВЕРДИТЬ";
        public string OpenValveGate { get; set; } = "Откройте затвор арматуры и подтвердите открытое состояние";
        public string CheckResult { get; set; } = "Проверьте арматуру и выберите результат";
    }
}
