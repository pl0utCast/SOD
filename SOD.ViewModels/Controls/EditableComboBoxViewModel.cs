using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Controls
{
    public class EditableComboBoxViewModel : ReactiveObject, IValueViewModel
    {
        public EditableComboBoxViewModel(IList<object> list, Func<Task<object>> addObjectGetter, string name)
        {
            Items = new ObservableCollection<object>(list);
            if (Items.Count > 0)
            {
                SelectedValue = Items.Last(); // забираем из конца списка значение, которое было выбрано при сохранении
                Items.Remove(Items.Last());
            }
            Add = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await addObjectGetter();
                if (result!=null)
                {
                    Items.Add(result);
                }
            });

            Delete = ReactiveCommand.Create(() =>
            {
                Items.Remove(SelectedValue);
            });
            Name = name;
            Prefix = name;
        }

        public string Name { get; set; }
        public string Prefix { get; set; }

        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        [Reactive]
        public object SelectedValue { get; set; }

        public ObservableCollection<object> Items { get; set; }
        public object GetValue()
        {
            Items.Add(SelectedValue); // выбранное значение добавляем в конец списка
            return Items.ToList();
        }
    }
}
