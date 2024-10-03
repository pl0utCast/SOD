using ReactiveUI;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using System.Reactive;

namespace SOD.ViewModels.Settings
{
    public class SettingsViewModel : ReactiveObject
    {
        private IDialogService dialogService;
        private ILocalizationService localizationService;

        public SettingsViewModel(INavigationService navigationService, ILocalizationService localizationService, IReportService reportService, IDialogService dialogService, App.Benches.CRSBench.Bench bench)
        {
            this.dialogService = dialogService;
            this.localizationService = localizationService;

            ViewTitle = localizationService["Settings.Settings"];
            GoTestBechSettings = ReactiveCommand.Create(() => navigationService.NavigateTo("TestBenchSettings"));
            GoIPSettings = ReactiveCommand.Create(() => navigationService.NavigateTo("IPSettingsView"));
            GoDeviceSensorSettings = ReactiveCommand.Create(() => navigationService.NavigateTo("DeviceAndSensorSettingsView"));
            GoBalloonSettings = ReactiveCommand.Create(() => navigationService.NavigateTo("BalloonSettings"));
            GoEditReportTemplate = ReactiveCommand.CreateFromTask(async() =>
            {
                var result = await reportService.EditReportTemplate(bench.Settings.ReportPath);

                if (result == 1)
                    dialogService.ShowMessage(localizationService["Settings.CantFindProtocol"]);
            });
            GoUsers = ReactiveCommand.Create(() => navigationService.NavigateTo("UserSettings"));
        }

        public string ViewTitle { get; set; } = "Настройки";
        public ReactiveCommand<Unit, Unit> GoTestBechSettings { get; set; }
        public ReactiveCommand<Unit, Unit> GoIPSettings { get; set; }
        public ReactiveCommand<Unit, Unit> GoDeviceSensorSettings { get; set; }
		public ReactiveCommand<Unit, Unit> GoBalloonSettings { get; set; }
		public ReactiveCommand<Unit, Unit> GoEditReportTemplate { get; set; }
        public ReactiveCommand<Unit, Unit> GoUsers { get; set; }
    }
}
