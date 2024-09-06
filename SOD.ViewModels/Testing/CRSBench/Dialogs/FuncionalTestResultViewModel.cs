using SOD.App.Testing.Funcional;
using SOD.Dialogs;
using SOD.LocalizationService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.CRSBench.Dialogs
{
    public class FuncionalTestResultViewModel : TestResultViewModel
    {
        public FuncionalTestResultViewModel(Result.PostResult result, ILocalizationService localizationService, IDialogService dialogService, IObservable<bool> canAdd) : base(dialogService, canAdd)
        {
            OpenPressure = result.OpenPressure.ToString("f2");
            ClosePressure = result.ClosePressure.ToString("f2");
            ExpectedSetPressure = result.ExpectedSetPressure.ToString("f2");
            Accuracy = result.Accuracy.ToString("f1");

            Add = ReactiveCommand.Create(() =>
            {
                result.Result = Valid ? localizationService["Testing.Test.Valid"] : localizationService["Testing.Test.UnValid"];
                dialogService.CloseAsync(true);
            }, canAdd);
        }
        public string OpenPressure { get; set; }
        public string ClosePressure { get; set; }
        public string ExpectedSetPressure { get; set; }
        public string Accuracy { get; set; }
        [Reactive]
        public bool Valid { get; set; } = true;
        [Reactive]
        public bool UnValid { get; set; }
    }
}
