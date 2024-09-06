using SOD.Core.Units;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using UnitsNet;

namespace SOD.ViewModels.Controls
{
    public class UnitValueViewModel : ReactiveObject, IValueViewModel
    {
        public UnitValueViewModel(IQuantity quantity, string prefix = "")
        {
            UnitTypes = quantity.GetUnitTypeInfo();
            SelectedUnitInfo = UnitTypes.SingleOrDefault(ui => ui.UnitType.ToString() == quantity.Unit.ToString());
            Value = (double)quantity.Value;
            Name = prefix;
            Prefix = prefix;
        }

        [Reactive]
        public UnitTypeInfo SelectedUnitInfo { get; set; }
        [Reactive]
        public double Value { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public IReadOnlyList<UnitTypeInfo> UnitTypes { get; set; }

        public object GetValue()
        {
            return UnitsHelper.GetValue(Value, SelectedUnitInfo);
        }
    }
}
