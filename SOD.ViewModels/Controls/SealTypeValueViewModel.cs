using SOD.Core.Seals;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Controls
{
    public class SealTypeValueViewModel : ReactiveObject, IValueViewModel
    {
        public SealTypeValueViewModel(SealType value, string name, string prefix)
        {
            Value = value;
            Prefix = prefix;
            Name = name;
        }

        public object GetValue()
        {
            return Value;
        }

        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public SealType Value { get; set; }
    }
}
