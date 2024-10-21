using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Testing.CRSBench
{
    public class CRSBench
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
		public string Tenso { get; set; } = "Вес";
		public string Time { get; set; } = "Время (чч:мм:сс)";
        public string SelectOpenPoint { get; set; } = "Выбрать точку открытия";
        public string SelectClosePoint { get; set; } = "Выбрать точку закрытия";
        public string Step1 { get; set; } = "Step 1: choose armature";
        public string Step2 { get; set; } = "Step 2: press start";
        public string Step3 { get; set; } = "Step 3: press test";
        public string Step4 { get; set; } = "Step 4: press second test";
        public string Step5 { get; set; } = "Step 5: press stop or test";
        public string Step6 { get; set; } = "Step 6: press result";
        public string Step6_2 { get; set; } = "Step 6: press result";

        public TestSettings TestSettings { get; set; } = new TestSettings();
        public TestParameters TestParameters { get; set; } = new TestParameters();
    }
}
