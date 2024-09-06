using SOD.Core.CustomUnits;
using SOD.Core.Props;
using SOD.Core.Seals;
using SOD.Core.Valves.Properties;
using SOD.ViewModels.Controls;
using UnitsNet;

namespace SOD.ViewModels.Extensions
{
    public static class PropertyExtension
    {
        public static IValueViewModel GetViewModel(this ValveProperty valvePropery, params object[] param)
        {
            if (valvePropery.Value is IQuantity quantity)
            {
                return new UnitValueViewModel(quantity, valvePropery.Prefix);
            }
            else if (valvePropery.Value is DinPressureClass dinPressureClass)
            {
                return new PressureClassViewModel(dinPressureClass, dinPressureClass.Unit, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Value is AnsiPressureClass ansiPressureClass)
            {
                return new PressureClassViewModel(ansiPressureClass, ansiPressureClass.Unit, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Type == PropertyType.String)
            {
                return new StringValueViewModel((string)valvePropery.Value, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Type == PropertyType.Double)
            {
                return new DoubleValueViewModel((double)valvePropery.Value, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Type == PropertyType.Integer)
            {
                return new IntegerValueViewModel((int)valvePropery.Value, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Value is NominalPipeSizeMetric nominalPipeSizeMetric)
            {
                return new NpsViewModel(nominalPipeSizeMetric, nominalPipeSizeMetric.Unit, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Value is NominalPipeSizeInch nominalPipeSizeInch)
            {
                return new NpsViewModel(nominalPipeSizeInch, nominalPipeSizeInch.Unit, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Type == PropertyType.StringList)
            {
                return new EditableComboBoxViewModel((IList<object>)valvePropery.Value, (Func<Task<object>>)param[0], (string)param[1]);
            }
            else if (valvePropery.Value is SealType st)
            {
                return new SealTypeValueViewModel(st, valvePropery.Name, valvePropery.Prefix);
            }
            else if (valvePropery.Value is TestMediumType tm)
            {
                return new TestMediumValueViewModel(tm, valvePropery.Name, valvePropery.Prefix);
            }
            return null;
        }

        public static IValueViewModel GetValueViewModel(this IProperty property, params object[] param)
        {
            if (property.Value is IQuantity quantity)
            {
                return new UnitValueViewModel(quantity, property.Name);
            }
            else if (property.Value is DinPressureClass dinPressureClass)
            {
                return new PressureClassViewModel(dinPressureClass, dinPressureClass.Unit, property.Name, property.Alias);
            }
            else if (property.Value is AnsiPressureClass ansiPressureClass)
            {
                return new PressureClassViewModel(ansiPressureClass, ansiPressureClass.Unit, property.Name, property.Alias);
            }
            else if (property.Type == PropertyType.String)
            {
                return new StringValueViewModel((string)property.Value, property.Name, property.Alias);
            }
            else if (property.Type == PropertyType.Double)
            {
                return new DoubleValueViewModel((double)property.Value, property.Name, property.Alias);
            }
            else if (property.Type == PropertyType.Integer)
            {
                return new IntegerValueViewModel((int)property.Value, property.Name, property.Alias);
            }
            else if (property.Value is NominalPipeSizeMetric nominalPipeSizeMetric)
            {
                return new NpsViewModel(nominalPipeSizeMetric, nominalPipeSizeMetric.Unit, property.Name, property.Alias);
            }
            else if (property.Value is NominalPipeSizeInch nominalPipeSizeInch)
            {
                return new NpsViewModel(nominalPipeSizeInch, nominalPipeSizeInch.Unit, property.Name, property.Alias);
            }
            else if (property.Type == PropertyType.StringList)
            {
                return new EditableComboBoxViewModel((IList<object>)property.Value, (Func<Task<object>>)param[0], (string)param[1]);
            }
            else if (property.Value is SealType st)
            {
                return new SealTypeValueViewModel(st, property.Name, property.Alias);
            }
            else if (property.Value is TestMediumType tm)
            {
                return new TestMediumValueViewModel(tm, property.Name, property.Alias);
            }
            return null;
        }

    }
}
