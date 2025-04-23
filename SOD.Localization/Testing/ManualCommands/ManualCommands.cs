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
        public string Command { get; set; } = "Команда";
        public string Test { get; set; } = "ИСПЫТАНИЕ";
        public string Name { get; set; } = "Название";
        public string ValveType { get; set; } = "Тип арматуры";
        public string EditingTestMethod { get; set; } = "Редактирование методики испытания";
        public string ControlDevice { get; set; } = "Управляющее устройство";
        public string SensorScript { get; set; } = "Скрипт выбора датчиков";

        public string FillingBalloon { get; set; } = "Заполнение баллона";
        public string EmptyingBalloon { get; set; } = "Опорожнение баллона";
        public string FillingCell { get; set; } = "Заполнение камеры";
        public string EmptyingCell { get; set; } = "Опорожнение камеры";
        public string PressureSet { get; set; } = "Набор давления";
        public string PressureRelease { get; set; } = "Сброс давления";
        public string VerticalCell { get; set; } = "Вертикальная камера";
        public string HorizontalCell { get; set; } = "Горизонтальная камера";

        public string PressureSetting { get; set; } = "Установка давления";
        public string PressureBar { get; set; } = "Давление (бар)";

        public string NoStandart { get; set; } = "Без стандарта";
        public string Standart { get; set; } = "Стандарт";
        public string ControlMethod { get; set; } = "Метод контроля";
        public string Confirm { get; set; } = "ПОДТВЕРДИТЬ";
    }
}
