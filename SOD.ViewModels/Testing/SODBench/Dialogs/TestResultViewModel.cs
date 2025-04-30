using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive;
using System.Text;
using SOD.App.Benches.SODBench;
using SOD.Core.Infrastructure;
using SOD.LocalizationService;
using SOD.App.Testing.Test;

namespace SOD.ViewModels.Testing.SODBench.Dialogs
{
	public class TestResultViewModel : ReactiveObject
	{
		public TestResultViewModel(App.Testing.Test.Result.PostResult result, ILocalizationService localizationService, IDialogService dialogService, IObservable<bool> canAdd)
		{
			OpenPressure = result.OpenPressure.ToString("f2");
			ClosePressure = result.ClosePressure.ToString("f2");
			ExpectedSetPressure = result.ExpectedSetPressure.ToString("f2");
			Accuracy = result.Accuracy.ToString("f1");

			Add = ReactiveCommand.Create(() =>
			{
				result.Result = Valid ? localizationService["Testing.Test.Valid"] : localizationService["Testing.Test.UnValid"];
				dialogService.CloseAsync(true);
			}, canAdd);

			Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(null));
		}

		public ReactiveCommand<Unit, Unit> Add { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }

		public string OpenPressure { get; set; }
		public string ClosePressure { get; set; }
		public string ExpectedSetPressure { get; set; }
		public string Accuracy { get; set; }
		public string Leakage { get; set; }
		[Reactive]
		public bool Valid { get; set; } = true;
		[Reactive]
		public bool UnValid { get; set; }
	}
}
