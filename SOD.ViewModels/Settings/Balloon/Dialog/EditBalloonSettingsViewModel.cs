using ReactiveUI;
using SOD.Dialogs;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Props;
using System.Reactive;
using SOD.ViewModels.Controls;

namespace SOD.ViewModels.Settings.Balloon.Dialog
{
	public class EditBalloonSettingsViewModel : YesNoDialogViewModel, IActivatableViewModel
	{
        public EditBalloonSettingsViewModel(IDialogService dialogService) : base(dialogService)
		{

			//this.WhenActivated(disposables =>
			//{
			//	//this.ValidationRule(
			//	//	viewModel => viewModel.Name,
			//	//	name => !string.IsNullOrWhiteSpace(name),
			//	//	"Поле не должно быть пустым!")
			//	//	.DisposeWith(disposables);

			//	//this.ValidationRule(
			//	//	viewModel => viewModel.Prefix,
			//	//	prefix => !string.IsNullOrWhiteSpace(prefix),
			//	//	"Поле не должно быть пустым!")
			//	//	.DisposeWith(disposables);
			//	//ValidationContext.DisposeWith(disposables);

			//	//this.ValidationRule(
			//	//	viewModel => viewModel.Prefix,
			//	//	prefix => !prefixs.Contains(prefix),
			//	//	"Данный prefix уже существует")
			//	//.DisposeWith(disposables);
			//});

			Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(false));
			Save = ReactiveCommand.Create(() => dialogService.CloseAsync(true));

		}
		public int Id { get; internal set; }
		[Reactive]
		public string Name { get; set; }
		[Reactive]
		public string Prefix { get; set; }
		[Reactive]
		public KeyValuePair<string, PropertyType> SelectedType { get; set; }
		public Dictionary<string, PropertyType> Types { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Save { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }
		
		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
