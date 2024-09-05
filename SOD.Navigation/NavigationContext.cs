using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SOD.Navigation
{
    public class NavigationContext<T> where T: FrameworkElement
    {
        public string RootKey { get; set; }
        public string ViewName { get; set; }
        public FrameworkElement View { get; set; }
        public Type ViewModelType { get; set; }
    }
}
