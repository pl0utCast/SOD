using SOD.Core.Infrastructure;
using SOD.App.Benches.CRSBench;
using SOD.App.Testing.Strength;
using SOD.Dialogs;
using SOD.LocalizationService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.CRSBench.Dialogs
{
    public class StrengthTestResultViewModel : TestResultViewModel, IActivatableViewModel
    {
        public StrengthTestResultViewModel(ISensorService sensorService,
                                           ILocalizationService localizationService,
                                           IDialogService dialogService,
                                           IObservable<bool> canAdd,
                                           Bench bench,
                                           Result.PostResult result) : base(dialogService, canAdd)
        {
            for (int i = 0; i < result.Registrations.Count; i++)
            {
                var exposure = new ExposureViewModel();
                exposure.ExposureNumber = i + 1;
                var pressureSensorResult = result.Registrations[i].StartPressure.FirstOrDefault();
                var pressureSensor = sensorService.GetSensor(pressureSensorResult.Id);
                exposure.StartPressure = result.Registrations[i].StartPressure.FirstOrDefault().Value.ToUnit(bench.Settings.PressureUnit).ToString(pressureSensor.Accaury);
                exposure.StopPressure = result.Registrations[i].StopPressure.FirstOrDefault().Value.ToUnit(bench.Settings.PressureUnit).ToString(pressureSensor.Accaury);
                Results.Add(exposure);
            }
            
            Add = ReactiveCommand.Create(() =>
            {
                for (int i = 0; i < result.Registrations.Count; i++)
                {
                    result.Registrations[i].Result = Results[i].Valid ? localizationService["Testing.Test.Valid"] : localizationService["Testing.Test.UnValid"];
                }
                dialogService.CloseAsync(true);
            }, canAdd);
        }

        public List<ExposureViewModel> Results { get; set; } = new List<ExposureViewModel>();
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
