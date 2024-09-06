using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Controls
{
    public class StringValueViewModel : ReactiveObject, IValueViewModel
    {
        public StringValueViewModel(string value, string name, string prefix)
        {
            Name = name;
            Prefix = prefix;
            Value = value;
        }
        public string Name { get; set; }
        public string Prefix { get; set; }
        [Reactive]
        public string Value { get; set; }
        public object GetValue() => Value;
    }
}
