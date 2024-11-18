namespace SOD.Localization.Testing.SODBench
{
    public class TestSettings
    {
        public string Ambient { get; set; } = "Среда испытания";
        public string PressureSensor { get; set; } = "Датчик давления";
        public string LeakageSensor { get; set; } = "Датчик протечки";
        public string Standart { get; set; } = "Стандарт";
        public string ExposureTime { get; set; } = "Время выдержки (секунды)";
        public string ExpectedSetPressure { get; set; } = "Ожидаемое давление открытия";
        public string UseValveSetPressure { get; set; } = "Из параметров арматуры";
		public string FullDeformation { get; set; } = "Допустимая полная объемная деформация, %";
		public string Deformation { get; set; } = "Допустимая остаточная объемная деформация, %";
		public string MaxDeformation { get; set; } = "Критическая объемная деформация, %";
		public string DeformationToolTip { get; set; } = "При превышении введенной критической объемной деформации, во время испытания возможно разрушение баллона.";
		public string BalloonType { get; set; } = "Тип баллона";
		public string Mode { get; set; } = "Режим испытания:";
		public string ManualMode { get; set; } = "Ручной";
		public string AutoMode { get; set; } = "Автоматический";
		public string Tenso5 { get; set; } = "5 кг";
		public string Tenso10 { get; set; } = "10 кг";
		public string Tenso30 { get; set; } = "30 кг";

		public string Pressure { get; set; } = "Испытательное давление баллона";
		public string BalloonValue { get; set; } = "Объем баллона, л";
		public string Parameters { get; set; } = "Параметры баллона";
	}
}