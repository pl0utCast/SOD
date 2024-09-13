using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using SOD.Core.Props;
using SOD.Dialogs;
using SOD.ViewModels.Controls;

namespace SOD.ViewModels.Props
{
	public class PropertyViewModel : ReactiveObject
	{
		public PropertyViewModel(IProperty property, IDialogService dialogService)
		{
			Name = property.Name;
			Alias = property.Alias;
			Type = property.Type;
			//if (property.Type == PropertyType.StringList)
			//{
			//    var vm = new Valves.Dialog.AddListStringItemViewModel(dialogService, property.Name);
			//    Func<Task<object>> func = () => dialogService.ShowDialogAsync("AddListStringItem", vm);
			//    Value = property.GetValueViewModel(func, property.Name);
			//}
			//else
			//Value = property.GetValueViewModel();
			Property = property;
		}

		public void Save()
		{
			Property.Alias = Alias;
			Property.Name = Name;
			//if (Property.Type != Type)
			//{
			Property.Type = Type;
			//    Property.Value = Type.GetDefaultValueByType();
			//}
			//else if (Property.Value == null)
			//{
			//    Property.Value = Type.GetDefaultValueByType();
			//}
		}
		[Reactive]
		public string Name { get; set; }
		[Reactive]
		public string Alias { get; set; }
		[Reactive]
		public PropertyType Type { get; set; }
		public IValueViewModel Value { get; set; }
		public IProperty Property { get; set; }
	}
}