using SOD.App.Benches.SODBench;
using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Dialogs
{
    public class AddTestDialogViewModel : ReactiveObject
    {
        //public AddTestDialogViewModel(IDialogService dialogService, List<TestSettings> testSettings)
        //{
        //    Tests = testSettings;
        //    Cancel = ReactiveCommand.Create(() => dialogService.CloseAsync(null));
        //    Save = ReactiveCommand.Create(() =>
        //    {
        //        var testConfig = new TestProgrammConfig();
        //        testConfig.TestType = Test.Type;
        //        testConfig.Name = Test.Name;
        //        dialogService.CloseAsync(testConfig);
        //    }, this.WhenAny(x=>x.Test, t=>t.Value!=null));
        //}
        //[Reactive]
        //public TestSettings Test { get; set; }
        //public List<TestSettings> Tests { get; set; }
        //public ReactiveCommand<Unit, Unit> Save { get; set; }
        //public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
