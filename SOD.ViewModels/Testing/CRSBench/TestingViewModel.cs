using MemBus;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Benches;
using SOD.App.Benches.CRSBench.Messages;
using SOD.Core.Cryptography;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.ViewModels.Controls;
using System.IO;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class TestingViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly DefaultTestingViewModel defaultTestingViewModel;
        private readonly FuncionalityTestViewModel funcionalityTestViewModel;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger logger = LogManager.GetLogger("Core");
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";

        public TestingViewModel(INavigationService navigationService,
                                ITestBenchService testBenchService,
                                IBus bus,
                                IDialogService dialogService,
                                ISensorService sensorService,
                                ISettingsService settingsService,
                                ILocalizationService localizationService)
        {
            ViewTitle = localizationService["MainView.Testing"];
            _dialogService = dialogService;
            _settingsService = settingsService;
            _localizationService = localizationService;

            var bench = (SOD.App.Benches.CRSBench.Bench)testBenchService.GetTestBench();
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());

            defaultTestingViewModel = new DefaultTestingViewModel(navigationService, testBenchService, bus, dialogService, sensorService, localizationService);
            funcionalityTestViewModel = new FuncionalityTestViewModel(navigationService, sensorService, bus, localizationService, dialogService, bench);

            CheckOldCalibrateFileAsync();

            this.WhenActivated(dis =>
            {
                if (bench.Settings.SelectedTestSettings?.Type == App.Testing.TestType.Functional)
                {
                    Test?.Activator.Deactivate();
                    Test = funcionalityTestViewModel;
                    funcionalityTestViewModel.Activator.Activate().DisposeWith(dis);
                }
                else
                {
                    Test?.Activator.Deactivate();
                    Test = defaultTestingViewModel;
                    defaultTestingViewModel.Activator.Activate().DisposeWith(dis);
                }

                bus.Subscribe<SelectedTestMessage>(m =>
                {
                    if (bench.Settings.SelectedTestSettings.Type == App.Testing.TestType.Functional)
                    {
                        if (Test.GetType() != typeof(FuncionalityTestViewModel))
                        {
                            Test?.Activator.Deactivate();
                        }
                        funcionalityTestViewModel.Activator.Activate().DisposeWith(dis);
                        Test = funcionalityTestViewModel;
                    }
                    else
                    {
                        if (Test.GetType() != typeof(DefaultTestingViewModel))
                        {
                            Test?.Activator.Deactivate();
                        }
                        defaultTestingViewModel.Activator.Activate().DisposeWith(dis);
                        Test = defaultTestingViewModel;
                    }
                    ViewTitle = bench.Settings.SelectedTestSettings?.LocalName;
                }).DisposeWith(dis);
            });
        }

        private async Task CheckOldCalibrateFileAsync()
        {
            if (LastUpadateSensorSettings == null) return;
            // Если последняя запись в калибровочный файл была больше 2-х лет назад
            if (LastUpadateSensorSettings.LastUpdateDate < DateTime.Now.AddYears(-2))
            {
                _dialogService.ShowMessage(_localizationService["MainView.LastUpdateDate"]);
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings", "sensorSettings.json");

            // Находим контрольную сумму файла
            int findCRCFromFile = new CRC(CRCCode.CRC32).FindCRC32(filePath);
            string checkSumPath = Path.Combine(Directory.GetCurrentDirectory(), "settings", "sensorSettingsCheckSum.json");

            try
            {
                using (StreamReader sr = new(checkSumPath))
                {
                    string checkSumFromFile = sr.ReadLine();

                    // Если последняя запись в калибровочный файл была больше 2-х лет назад
                    if (checkSumFromFile != findCRCFromFile.ToString("X2"))
                    {
                        var result = await _dialogService.ShowDialogAsync("CheckSumError", new YesNoDialogViewModel(_dialogService));
                        if ((bool)result)
                        {
                            var backupPath = Path.Combine(Directory.GetCurrentDirectory(), "settings", "backup_sensorSettings.json");

                            if (File.Exists(backupPath))
                            {
                                File.Delete(filePath);
                                File.Copy(backupPath, filePath);

                                // Перезапускаем ПО
                                System.Windows.Forms.Application.Restart();
                                await Task.Delay(1000);
                                Environment.Exit(0);
                            }
                            else
                            {
                                _dialogService.ShowMessage(_localizationService["Testing.CRSBench.BackupFileError"]);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                logger.Warn(string.Format("Ошибка чтения файла - {0}", checkSumPath));
            }


            //if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png")))
            //{
            //    DateTime time = File.GetLastWriteTime(Path.Combine(Directory.GetCurrentDirectory(), "reports_template", "qr.png"));
            //    if (time < DateTime.Now.AddYears(-2))
            //    {
            //        _dialogService.ShowMessage(_localizationService["MainView.LastUpdateDate"]);
            //    }
            //}
        }

        [Reactive]
        public string ViewTitle { get; set; } = "Испытания";
        [Reactive]
        public IActivatableViewModel Test { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
