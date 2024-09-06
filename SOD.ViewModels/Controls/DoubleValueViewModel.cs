using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Controls
{
    public class DoubleValueViewModel : ReactiveObject, IValueViewModel
    {
        public DoubleValueViewModel(double value, string name, string prefix)
        {
            Name = name;
            Prefix = prefix;
            Value = value;
        }
        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public double Value { get; set; }
        public object GetValue() => Value;
    }
}
