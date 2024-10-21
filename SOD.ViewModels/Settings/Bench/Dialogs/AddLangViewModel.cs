using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using SOD.Dialogs;
using SOD.ViewModels.Controls;
using System.Globalization;

namespace SOD.ViewModels.Settings.Bench.Dialogs
{
	public class AddLangViewModel : YesNoDialogViewModel
	{
		public AddLangViewModel(IDialogService dialogService) : base(dialogService)
		{
			Langs.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
			Yes = ReactiveCommand.Create(() => dialogService.CloseAsync(CurrentLang));
			No = ReactiveCommand.Create(() => dialogService.CloseAsync(null));
		}

		public List<CultureInfo> Langs { get; set; } = new List<CultureInfo>();
		[Reactive]
		public CultureInfo CurrentLang { get; set; }
	}
}
