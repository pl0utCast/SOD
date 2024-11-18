using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Balloons.Properties;
using SOD.Core.Device.Modbus;
using SOD.Core.Reports;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.ViewModels.Settings.DeviceAndSensor.Device;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using static SOD.App.Benches.SODBench.Report.ReportData;
using BalloonProperty = SOD.Core.Balloons.Properties.BalloonProperty;

namespace SOD.ViewModels.Settings.Balloon
{
	public class BalloonSettingsViewModel : ReactiveObject, IActivatableViewModel
	{
		public BalloonSettingsViewModel(IDialogService dialogService, 
										ILocalizationService localizationService,
										INavigationService navigationService,
										App.Benches.SODBench.Bench bench)
		{

			BalloonProperties.AddRange(bench.Settings.BalloonProperties);

			Add = ReactiveCommand.CreateFromTask(async () =>
			{
				var vm = new Dialog.EditBalloonSettingsViewModel(dialogService)
				{
					Prefix = String.Empty,
					Name = string.Empty,
					Types = PropertyHelper.GetPropertyTypeDictonary()
				};
				vm.SelectedType = vm.Types.SingleOrDefault(kv => kv.Value == Core.Props.PropertyType.String);
				if ((bool)await dialogService.ShowDialogAsync("EditBalloonProperty", vm))
				{
					var newprop = new BalloonProperty()
					{
						Name = vm.Name,
						Prefix = vm.Prefix,
						Type = vm.SelectedType.Value,
						Id = BalloonProperties.Last().Id + 1
					};
					BalloonProperties.Add(newprop);
					bench.Settings.BalloonProperties.Add(newprop);
					bench.SaveSettings();					
				}
			});

			this.WhenActivated(disposables =>
			{
				Edit = ReactiveCommand.CreateFromTask(async () =>
				{
					var vm = new Dialog.EditBalloonSettingsViewModel(dialogService)
					{
						Id = SelectedProperty.Id,
						Prefix = SelectedProperty.Prefix,
						Name = SelectedProperty.Name,
						Types = PropertyHelper.GetPropertyTypeDictonary(),
						SelectedType  = PropertyHelper.GetPropertyTypeDictonary().SingleOrDefault(x => x.Value == SelectedProperty.Type)						
					};
					var result = await dialogService.ShowDialogAsync("EditBalloonProperty", vm);
					if ((bool)result)
					{
						SelectedProperty.Name = vm.Name;
						SelectedProperty.Prefix = vm.Prefix;
						SelectedProperty.Type = vm.SelectedType.Value;
						bench.Settings.BalloonProperties = BalloonProperties.ToList();
						bench.SaveSettings();
						BalloonProperties.Clear();
						BalloonProperties.Add(bench.Settings.BalloonProperties);
						dialogService.ShowMessage("Данные успешно сохранены!");
					}
				}, this.WhenAny(x => x.SelectedProperty, (sp) => sp.Value != null)).DisposeWith(disposables);

				Delete = ReactiveCommand.Create(() =>
				{
					bench.Settings.BalloonProperties.Remove(SelectedProperty);
					BalloonProperties.Remove(SelectedProperty);
					bench.SaveSettings();
				}, this.WhenAny(x => x.SelectedProperty, (sp) => sp.Value != null)).DisposeWith(disposables);
			});
			GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
			
		}

		[Reactive]
		public string Name { get; set; }
		[Reactive]
		public string Prefix { get; set; }

		public ObservableCollection<BalloonProperty> BalloonProperties { get; set; } = [];
		[Reactive]
		public BalloonProperty SelectedProperty { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> GoBack { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Save { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Add { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Delete { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Edit { get; set; }

		public ViewModelActivator Activator { get; } = new ViewModelActivator();
			
	}
}
