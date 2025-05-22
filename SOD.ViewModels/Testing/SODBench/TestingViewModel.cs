using MemBus;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Benches;
using SOD.App.Benches.SODBench.Messages;
using SOD.Core.Cryptography;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Dialogs;
using SOD.Localization;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.ViewModels.Controls;
using SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands;
using System.IO;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Testing.SODBench
{
	public class TestingViewModel : ReactiveObject, IActivatableViewModel
	{
		private readonly DefaultTestingViewModel defaultTestingViewModel;
		private readonly IDialogService _dialogService;
		private readonly ISettingsService _settingsService;
		private readonly ILocalizationService _localizationService;
		private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";

		public TestingViewModel(INavigationService navigationService,
								ITestBenchService testBenchService,
								IBus bus,
								IDialogService dialogService,
								IDeviceService deviceService,
								ISensorService sensorService,
								ISettingsService settingsService,
								ILocalizationService localizationService)
		{
			ViewTitle = localizationService["MainView.Testing"];
			_dialogService = dialogService;
			_settingsService = settingsService;
			_localizationService = localizationService;

			var bench = (SOD.App.Benches.SODBench.Bench)testBenchService.GetTestBench();
			LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());

			defaultTestingViewModel = new DefaultTestingViewModel(navigationService, testBenchService, bus, dialogService, sensorService, localizationService);

			this.WhenActivated(dis =>
			{
				Test?.Activator.Deactivate();
				Test = defaultTestingViewModel;
				defaultTestingViewModel.Activator.Activate().DisposeWith(dis);

                Commands.Activator.Activate().DisposeWith(dis);

                bus.Subscribe<SelectedTestMessage>(m =>
				{
					defaultTestingViewModel.Activator.Activate().DisposeWith(dis);
					Test = defaultTestingViewModel;
					//ViewTitle = bench.Settings.SelectedTestSettings?.LocalName;
				}).DisposeWith(dis);
			});

            Commands = new CommandsViewModel(bus, dialogService, deviceService, _localizationService);
        }

		[Reactive]
		public string ViewTitle { get; set; } = "Испытания";
		[Reactive]
		public IActivatableViewModel Test { get; set; }
        public CommandsViewModel Commands { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
