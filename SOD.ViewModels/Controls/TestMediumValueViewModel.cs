using SOD.Core.Seals;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Controls
{
    public class TestMediumValueViewModel : ReactiveObject, IValueViewModel
    {
        public TestMediumValueViewModel(TestMediumType value, string name, string prefix)
        {
            Value = value;
            Name = name;
            Prefix = prefix;
        }

        public object GetValue()
        {
            return Value;
        }
        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public TestMediumType Value { get; set; }
    }
}
