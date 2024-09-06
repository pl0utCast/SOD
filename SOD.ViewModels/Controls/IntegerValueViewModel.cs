using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Controls
{
    public class IntegerValueViewModel : ReactiveObject, IValueViewModel
    {
        public IntegerValueViewModel(int value, string name, string prefix)
        {
            Name = name;
            Prefix = prefix;
            Value = value;
        }
        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public int Value { get; set; }
        public object GetValue() => Value;
    }
}
