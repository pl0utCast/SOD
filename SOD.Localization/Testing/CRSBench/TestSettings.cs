namespace SOD.Localization.Testing.CRSBench
{
    public class TestSettings
    {
        public string Ambient { get; set; } = "Среда испытания";
        public string PressureSensor { get; set; } = "Датчик давления";
        public string LeakageSensor { get; set; } = "Датчик протечки";
        public string Standart { get; set; } = "Стандарт";
        public string ExposureTime { get; set; } = "Время испытания (секунды)";
        public string ExpectedSetPressure { get; set; } = "Ожидаемое давление открытия";
        public string UseValveSetPressure { get; set; } = "Из параметров арматуры";
        public string LeakageHint { get; set; } = "Протечка";

    }
}