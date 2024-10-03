using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Props;
using SOD.Dialogs;
using SOD.UserService;
using SOD.ViewModels.Props;
using SOD.ViewModels.Props.Dialogs;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Settings.Bench.CRSBench
{
	public class ParametersSettingsViewModel : ReactiveObject, IActivatableViewModel
	{
		private readonly App.Benches.CRSBench.Settings _settings;
		public ParametersSettingsViewModel(IDialogService dialogService, App.Benches.CRSBench.Settings settings, IUserService userService)
		{
			if (userService.GetCurrentUser().Role == UserRole.Administrator || userService.GetCurrentUser().Role == UserRole.Technologist)
				ItemFromTehnologist = true;

			_settings = settings;
			foreach (var param in settings.Parameters)
			{
				Parameters.Add(new PropertyViewModel(param, dialogService));
			}

			this.WhenActivated(dis =>
			{
				Add = ReactiveCommand.CreateFromTask(async () =>
				{
					var propVm = new PropertyViewModel(new Property(), dialogService);
					var dialogVm = new EditPropertyViewModel(dialogService, propVm, Parameters);
					var result = await dialogService.ShowDialogAsync("EditProperty", dialogVm);
					if (result is bool isAddOrEdit && isAddOrEdit)
					{
						Parameters.Add(propVm);
					}
				})
				.DisposeWith(dis);

				Edit = ReactiveCommand.Create(() =>
				{
					var dialogVm = new EditPropertyViewModel(dialogService, Parameter, Parameters);
					dialogService.ShowDialog("EditProperty", dialogVm);
				}, this.WhenAnyValue(x => x.Parameter).Select(p => p != null))
				.DisposeWith(dis);

				Delete = ReactiveCommand.CreateFromTask(async () =>
				{
					var result = await dialogService.ShowDialogAsync("DeleteProperty", new DeleteParameterViewModel(dialogService));
					if ((bool)result)
					{
						Parameters.Remove(Parameter);
					}
				}, this.WhenAnyValue(x => x.Parameter).Select(p => p != null))
				.DisposeWith(dis);
			});

		}
		public void Save()
		{
			_settings.Parameters.Clear();
			foreach (var parameter in Parameters)
			{
				parameter.Save();
				_settings.Parameters.Add((Property)parameter.Property);
			}
		}
		[Reactive]
		public ReactiveCommand<Unit, Unit> Add { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Edit { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Delete { get; set; }
		[Reactive]
		public PropertyViewModel Parameter { get; set; }
		public ObservableCollection<PropertyViewModel> Parameters { get; set; } = new ObservableCollection<PropertyViewModel>();

		public ViewModelActivator Activator { get; } = new ViewModelActivator();

		public bool ItemFromTehnologist { get; set; }
	}
}
