using System;

namespace SOD.Localization
{
    public class Localization
    {
        public MainView MainView { get; set; } = new MainView();
        public MediumType MediumType { get; set; } = new MediumType();
        public ControlType ControlType { get; set; } = new ControlType();
        public SealType SealType { get; set; } = new SealType();
        public string Title { get; set; } = "Главное окно";
        public SODBenchLocalization SODBenchLocalization { get; set; } = new SODBenchLocalization();

        public PropsLang Props { get; set; } = new PropsLang();
        public ReportsLang Reports { get; set; } = new ReportsLang();
        public SettingsLang Settings { get; set; } = new SettingsLang();
        public TestingLang Testing { get; set; } = new TestingLang();
        public Valves Valves { get; set; } = new Valves();
        public Prefixes Prefixes { get; set; } = new Prefixes();
        public VideoLang Video { get; set; } = new VideoLang();

        public LoginLang Login { get; set; } = new LoginLang();
        public ExitLang Exit { get; set; } = new ExitLang();

        public class PropsLang
        {
            public Props.DeletePropView DeletePropView { get; set; } = new Props.DeletePropView();
            public Props.EditPropertyView EditPropertyView { get; set; } = new Props.EditPropertyView();
        }

        public class ReportsLang
        {
            public string Reports { get; set; } = "Протоколы";
            public Reports.ReportsView ReportsView { get; set; } = new Reports.ReportsView();
            public Reports.SavedReportPanelView SavedReportPanelView { get; set; } = new Reports.SavedReportPanelView();
            public Reports.SavedReportsView SavedReportsView { get; set; } = new Reports.SavedReportsView();
            public Reports.DeleteReportsDialogView DeleteReportsDialogView { get; set; } = new Reports.DeleteReportsDialogView();
            public Reports.ExportReportDialogView ExportReportDialogView { get; set; } = new Reports.ExportReportDialogView();
        }

        public class SettingsLang
        {
            public string Settings { get; set; } = "Настройки";
            public string SystemSettings { get; set; } = "Настройки системы";
            public string GenerateQrCode { get; set; } = "Генерация QR-кода";
            public string ConfigIpAddress { get; set; } = "Настройка IP-адреса";
            public string ValveType { get; set; } = "Виды арматуры";
            public string Standart { get; set; } = "Стандарты";
            public string DevicesAndSensors { get; set; } = "Устройства и датчики";
			public string BalloonSettings { get; set; } = "Свойства баллонов";
			public string EditLocalizationFile { get; set; } = "Редактировать файл перевода";
            public string EditReportTemplate { get; set; } = "Редактировать шаблон протокола";
            public string Users { get; set; } = "Пользователи";
            public string Themes { get; set; } = "Темы";
            public string BackgroundChanged { get; set; } = "Фон успешно изменен!";
            public string CantFindProtocol { get; set; } = "Не найден шаблон протокола";
            public BenchLang Bench { get; set; } = new BenchLang();
            public DeviceAndSensorLang DeviceAndSensor { get; set; } = new DeviceAndSensorLang();
            public Settings.QrCode.QrCode QrCode { get; set; } = new Settings.QrCode.QrCode();
            public Settings.IPSettings.IPSettings IPSettings { get; set; } = new Settings.IPSettings.IPSettings();
            public Settings.Standarts.Standarts Standarts { get; set; } = new Settings.Standarts.Standarts();
            public Settings.Valve.Valve Valve { get; set; } = new Settings.Valve.Valve();
            public Settings.Users UserSettings { get; set; } = new Settings.Users();
            public class BenchLang
            {
                public Settings.Bench.SodBench.SodBenchSettingsView SodBenchSettingsView { get; set; } = new Settings.Bench.SodBench.SodBenchSettingsView();
                public Settings.Bench.TestBenchSettingsView TestBenchSettingsView { get; set; } = new Settings.Bench.TestBenchSettingsView();
            }

            public class DeviceAndSensorLang
            {
                public string Devices { get; set; } = "Устройства";
                public string PressureSensors { get; set; } = "Датчики давления";
                public string LeakageSensors { get; set; } = "Датчики протечки";
                public string TemperatureSensors { get; set; } = "Датчики температуры";
				public string TensoSensors { get; set; } = "Тензодатчики";
				public Settings.DeviceAndSensors.Device Device { get; set; } = new Settings.DeviceAndSensors.Device();
                public Settings.DeviceAndSensors.Sensor Sensor { get; set; } = new Settings.DeviceAndSensors.Sensor();
            }
        }

        public class TestingLang
        {
            public string ValveName { get; set; } = "Имя арматуры";
            public string ClearFilters { get; set; } = "Очистить фильтр";
            public SOD.Localization.Testing.SODBench.SODBench SODBench { get; set; } = new SOD.Localization.Testing.SODBench.SODBench();
            public Testing.Test Test { get; set; } = new Testing.Test();
        }

        public class VideoLang
        {
            public string VideoName { get; set; } = "Видео";
            public string Name { get; set; } = "Имя";
            public string OnvifAddress { get; set; } = "Onvif адрес";
            public string OnvifUsername { get; set; } = "Onvif логин";
            public string OnvifPassword { get; set; } = "Onvif пароль";
            public string MainStreamAddress { get; set; } = "Главный поток";
            public string LowStreamAddress { get; set; } = "Вторичный поток";
            public string Position { get; set; } = "Позиция";
            public string Add { get; set; } = "Добавить";
            public string Delete { get; set; } = "Удалить";
            public string OpenWebCamera { get; set; } = "Открыть веб-страницу";
            public string DeletePosition { get; set; } = "Удалить позицию";
            public string Yes { get; set; } = "Да";
            public string No { get; set; } = "Нет";
            public string FAQ { get; set; } = "Управление с помощью клавиатуры:\nW,A,S,D - перемещение, Увеличение зума - Z, Уменьшение зума - X,\nЗапись - R, Остановка записи - T, Полный экран - F,\nИзменение активной камеры - K, L";
            public string Disconnected { get; set; } = "Нет соединения";
            public string VideoRecordingTime { get; set; } = "Время записи видео(мин)";
            public string DeleteVideoDays { get; set; } = "Старое видео, если ему(дней)";
            public string DeleteVideoOversize { get; set; } = "Удалять старые видео при памяти меньше(Гб)";
            public string CameraSettings { get; set; } = "Настройки камеры";
            public string GeneralSettings { get; set; } = "Общие настройки";
            public string VideoPathDoesNotExist { get; set; } = "Указанного пути для видеозаписи не существует!";
            public string MustSelect { get; set; } = "*Необходимо выбрать хотя бы одну камеру";
            public string UsedCameras { get; set; } = "Используемые камеры";
            public string Width { get; set; } = "Ширина окна";
            public string Height { get; set; } = "Высота окна";
            public string SetSizeAllCameras { get; set; } = "Установить размер для всех камер";
        }

        public class LoginLang
        {
            public string User { get; set; } = "Пользователь";
            public string Password { get; set; } = "Пароль";
            public string Enter { get; set; } = "ВХОД";
            public string Message { get; set; } = "Сообщение";
            public string IncorrPass { get; set; } = "Неверный пароль";
        }

        public class ExitLang
        {
            public string Reset { get; set; } = "Перезагрузка ПК";
            public string Shutdown { get; set; } = "Выключение ПК";
            public string Exit { get; set; } = "Закрыть программу";
            public string LogOut { get; set; } = "Сменить пользователя";
        }
    }

    
}
