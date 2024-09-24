using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;
using SOD.Core.Props;
using SOD.Dialogs;
using SOD.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Props.Dialogs
{
	public class EditPropertyViewModel : ReactiveValidationObject, IActivatableViewModel
	{
		private string oldName;
		private string oldAlias;
		private IValueViewModel oldValue;
		private PropertyType oldType;

		public EditPropertyViewModel(IDialogService dialogService, PropertyViewModel propertyViewModel, IReadOnlyList<PropertyViewModel> propertyViewModels)
		{
			Property = propertyViewModel;
			oldName = propertyViewModel.Name;
			oldAlias = propertyViewModel.Alias;
			oldType = propertyViewModel.Type;

			Name = propertyViewModel.Name;
			Alias = propertyViewModel.Alias;
			Type = propertyViewModel.Type;
			Types = Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>().ToList();

			this.WhenActivated(dis =>
			{
				this.ValidationRule(x => x.Name, n => !string.IsNullOrEmpty(n), "Не может быть пустым")
					.DisposeWith(dis);

				var validateAlias = this.WhenAnyValue(x => x.Alias)
					.Select(a =>
					{
						if (string.IsNullOrEmpty(a)) return new ValidationState(false, "Поле не должно быть пустым");
						if (propertyViewModels.Select(p => p.Alias).Where(a => a != oldAlias).Contains(a)) return new ValidationState(false, "Данный alias уже занят");
						return ValidationState.Valid;
					});
				this.ValidationRule(x => x.Alias, validateAlias)
					.DisposeWith(dis);
			});

			Save = ReactiveCommand.Create(() =>
			{
				propertyViewModel.Name = Name;
				propertyViewModel.Alias = Alias;
				propertyViewModel.Type = Type;
				dialogService.CloseAsync(true);
			}, ValidationContext.Valid);

			Cancel = ReactiveCommand.Create(() =>
			{
				propertyViewModel.Name = oldName;
				propertyViewModel.Alias = oldAlias;
				propertyViewModel.Type = oldType;
				dialogService.CloseAsync(false);
			});
		}
		[Reactive]
		public string Name { get; set; }
		[Reactive]
		public string Alias { get; set; }
		[Reactive]
		public PropertyType Type { get; set; }
		public List<PropertyType> Types { get; set; }
		public PropertyViewModel Property { get; set; }
		public ReactiveCommand<Unit, Unit> Save { get; set; }
		public ReactiveCommand<Unit, Unit> Cancel { get; set; }
		public ViewModelActivator Activator { get; } = new ViewModelActivator();
	}
}
