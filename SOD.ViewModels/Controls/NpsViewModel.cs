using SOD.Core.Units;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.ViewModels.Controls
{
    public class NpsViewModel : ReactiveObject, IValueViewModel
    {
        public NpsViewModel(object pressureClass, Enum unit, string name, string prefix)
        {
            Items = UnitsHelper.GetUnitTypeInfo(pressureClass.GetType(), pressureClass);
            SelectedItem = Items.SingleOrDefault(ui => ui.UnitType.ToString() == unit.ToString());
            Name = name;
            Prefix = prefix;
        }
        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public UnitTypeInfo SelectedItem { get; set; }
        public IReadOnlyList<UnitTypeInfo> Items { get; set; }

        public object GetValue()
        {
            return UnitsHelper.GetValue(SelectedItem);
        }
    }
}
