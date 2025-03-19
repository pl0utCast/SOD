using DynamicData;
using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Benches;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.Keyboard;
using SOD.Localization.Testing.SODBench;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.UserService;
using SOD.ViewModels.Settings.Bench.SODBench;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Settings.Bench
{
	public class TestBenchSettingsViewModel : ReactiveObject, IActivatableViewModel
	{
		private SODBench.SODBenchSettingsViewModel sodBenchSettingsViewModel;
		public TestBenchSettingsViewModel(
			INavigationService navigationService,
			ITestBenchService testBenchService,
			IDeviceService deviceService,
			ISensorService sensorService,
			ISettingsService settingsService,
			IDialogService dialogService,
			ILocalizationService localizationService,
			App.Benches.SODBench.Bench bench,
			IBus bus,
			IUserService userService)
		{
			ViewTitle = localizationService["Settings.SystemSettings"];
			Langs.AddRange(localizationService.SupportCulture);
			CurrentLang = localizationService.CurrentCulture;
			IsEnableVirtualKeyboard = VirtualKeyboard.Instance.IsEnableVirtualKeyboard;
			SettingsKeyboard = settingsService.GetSettings("VirtualKeyboard", new Keyboard.Settings());

			this.WhenActivated(dis =>
			{
				if (sodBenchSettingsViewModel == null)
				{
					var bench = (App.Benches.SODBench.Bench)testBenchService.GetTestBench();
                    sodBenchSettingsViewModel = new SODBench.SODBenchSettingsViewModel(dialogService, sensorService, bench, userService, settingsService, localizationService, deviceService);
                    sodBenchSettingsViewModel.Activator
											 .Activate()
											 .DisposeWith(dis);
				}
				CurrentBench = sodBenchSettingsViewModel;
			});


			GoBack = ReactiveCommand.Create(() => navigationService.GoBack());

			Save = ReactiveCommand.Create(() =>
			{
				localizationService.SupportCulture.Clear();
				localizationService.SupportCulture.AddRange(Langs);
				localizationService.CurrentCulture = CurrentLang;
				localizationService.SaveSettings();
				VirtualKeyboard.Instance.IsEnableVirtualKeyboard = IsEnableVirtualKeyboard;
				SettingsKeyboard.IsEnable = IsEnableVirtualKeyboard;
				settingsService.SaveSettings("VirtualKeyboard", SettingsKeyboard);
				CurrentBench?.Save();
				testBenchService.SaveSettings();
			});

			AddCulture = ReactiveCommand.CreateFromTask(async () =>
			{
				var result = await dialogService.ShowDialogAsync("AddLangView", new Dialogs.AddLangViewModel(dialogService));
				if (result != null && result is CultureInfo cultureInfo)
				{
					if (!Langs.Contains(cultureInfo)) Langs.Add(cultureInfo);
				}
			});
			DeleteCulture = ReactiveCommand.Create(() =>
			{
				Langs.Remove(CurrentLang);
			}, this.WhenAnyValue(x => x.CurrentLang).Select(cl => cl != null));

		}


		[Reactive]
		public bool IsEnableVirtualKeyboard { get; set; }
		public Keyboard.Settings SettingsKeyboard { get; }
		public ObservableCollection<CultureInfo> Langs { get; set; } = new ObservableCollection<CultureInfo>();
		[Reactive]
		public CultureInfo CurrentLang { get; set; }
		[Reactive]
		public BenchesType SelectedBench { get; set; }
		[Reactive]
		public SODBenchSettingsViewModel CurrentBench { get; set; }
		public ReactiveCommand<Unit, Unit> GoBack { get; set; }
		public ReactiveCommand<Unit, Unit> Save { get; set; }
		public ReactiveCommand<Unit, Unit> AddCulture { get; set; }
		public ReactiveCommand<Unit, Unit> DeleteCulture { get; set; }

		public string ViewTitle { get; set; } = "Настройки";

		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
