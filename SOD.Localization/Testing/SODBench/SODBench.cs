using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Testing.SODBench
{
    public class SODBench
    {
        public string CanSaveReport { get; set; } = "Текущий протокол не сохранён, сохранить?";
        public string No { get; set; } = "Нет";
        public string Save { get; set; } = "Сохранить";
        public string CheckSumError { get; set; } = "Обнаружено несоответствие файла калибровки!!! Выполнить восстановление последней сохраненной версии файла? Программа будет перезапущена.";
        public string StartPressure { get; set; } = "Начальное давление";
        public string StopPressure { get; set; } = "Конечное давление";
        public string OpenPressure { get; set; } = "Открытие";
        public string ClosePressure { get; set; } = "Закрытие";
        public string ExpectedSetPressure { get; set; } = "Ожидаемое";
        public string Leakage { get; set; } = "Протечка";
        public string Drops { get; set; } = "капель/пузырьков";
        public string TestResult { get; set; } = "Результат испытания";
        public string ExposureNumber { get; set; } = "Выдержка №";
        public string Valid { get; set; } = "Годная";
        public string UnValid { get; set; } = "Негодная";
        public string Results { get; set; } = "Результаты";
        public string AddToReport { get; set; } = "ДОБАВИТЬ В ПРОТОКОЛ";
        public string Cancel { get; set; } = "Отмена";
        public string ExposureTime { get; set; } = "Время выдержки";
        public string TempWater { get; set; } = "Температура воды";
        public string TempAir { get; set; } = "Температура воздуха";
        public string Exposure { get; set; } = "ВЫДЕРЖКА";
        public string Parameters { get; set; } = "ПАРАМЕТРЫ";
        public string Result { get; set; } = "РЕЗУЛЬТАТ";
        public string Start { get; set; } = "СТАРТ";
        public string Stop { get; set; } = "СТОП";
        public string Pressure { get; set; } = "Давление";
		public string Tenzo { get; set; } = "Усилие";
		public string Time { get; set; } = "Время (чч:мм:сс)";
        public string SelectOpenPoint { get; set; } = "Выбрать точку открытия";
        public string SelectClosePoint { get; set; } = "Выбрать точку закрытия";
        public string TotalDefBalloon { get; set; } = "Полная деф. баллона";
        public string ResidualDefBalloon { get; set; } = "Остаточная деф. баллона";
        public string Liter { get; set; } = "литр";
        public string AlAirKip { get; set; } = "Нет управляющего воздуха";
        public string AlStopDropP { get; set; } = "К. стоп сброс P";
        public string AlStop { get; set; } = "К. стоп остановки";
        public string AlE1 { get; set; } = "E1 Обрыв датчика";
        public string AlDiscET7026 { get; set; } = "Обрыв связи с модулем ET-7026";
        public string AlDiscET7026_1 { get; set; } = "Обрыв связи с модулем ET-7026_1";
        public string AlDiscET7026_2 { get; set; } = "Обрыв связи с модулем ET-7026_2";
        public string AlPDown { get; set; } = "Резкое падение давления";
        public string AlNomP { get; set; } = "Превышение номинального давления";
        public string AlMaxP { get; set; } = "Превышение максимального давления";
        public string AlLongTimeStable { get; set; } = "Превышено время ожидания стабилизации веса E1";
        public string AlLevelHight { get; set; } = "Уровень в колбе превысил норму";
        public string AlHi5kg { get; set; } = "Высокий уровень в колбе 5кг";
        public string AlHi10kg { get; set; } = "Высокий уровень в колбе 10кг";
        public string AlHi30kg { get; set; } = "Высокий уровень в колбе 30кг";
        public string MessReseptOk { get; set; } = "Рецепт корректен";
        public string MessTestOk { get; set; } = "Испытания по выходу разрешены";
        public string MessPDecrOk { get; set; } = "P сброшено после превышения";
        public string MessGoodCylinder { get; set; } = "Баллон соответсвует";
        public string MessBadCylinder { get; set; } = "Баллон не соответсвует";
        public string RefreshC { get; set; } = "Обновлено";
        public string Step1 { get; set; } = "Step 1: choose armature";

        public TestSettings TestSettings { get; set; } = new TestSettings();
        public TestParameters TestParameters { get; set; } = new TestParameters();
    }
}
