using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor
{
    public interface ISupportSave: IObservable<bool>
    {
        bool CanSave { get; }
        public void Save();
    }
}
