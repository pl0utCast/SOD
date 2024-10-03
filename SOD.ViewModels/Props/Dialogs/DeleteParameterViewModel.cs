using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using SOD.Dialogs;
using System.Reactive;
using SOD.ViewModels.Controls;

namespace SOD.ViewModels.Props.Dialogs
{
	public class DeleteParameterViewModel : YesNoDialogViewModel, IActivatableViewModel
	{
		public DeleteParameterViewModel(IDialogService dialogService) : base(dialogService)
		{
			Delete = ReactiveCommand.Create(() => dialogService.CloseAsync(true));
			Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(false));
		}
		[Reactive]
		public ReactiveCommand<Unit, Unit> Delete { get; set; }
		[Reactive]
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }

		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
