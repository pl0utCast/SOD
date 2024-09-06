using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.App.Benches.CRSBench;
using SOD.App.Testing.Leakage;
using SOD.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.CRSBench.Dialogs
{
    public class LeakageTestResultViewModel : TestResultViewModel
    {
        public LeakageTestResultViewModel(ISensorService sensorService, IDialogService dialogService, IObservable<bool> canAdd, Bench bench, Result testResult): base(dialogService, canAdd)
        {
            for (int i = 0; i < testResult.PostResults.FirstOrDefault().Registrations.Count; i++)
            {
                var registration = testResult.PostResults.FirstOrDefault().Registrations[i];
                var exposure = new ExposureViewModel();
                exposure.ExposureNumber = i + 1;
                if (bench.Settings.LeakageUnit == UnitsNet.Units.VolumeFlowUnit.CubicCentimeterPerMinute)
                {
                    exposure.Leakage = registration.Leakage.FirstOrDefault().Value.ToUnit(bench.Settings.LeakageUnit).ToString("f2");
                }
                else
                {
                    var sensorResult = registration.Leakage.FirstOrDefault();
                    var sensor = (ILeakageSensor)sensorService.GetSensor(sensorResult.Id);
                    exposure.Leakage = sensorResult.Value.ToUnit(bench.Settings.LeakageUnit).ToString(sensor.Accaury);
                }
                if (registration.Drops.Count > 0)
                {
                    exposure.Drops = registration.Drops.FirstOrDefault().Value.ToString();
                }
                var pressureSensorResult = registration.StartPressure.FirstOrDefault();
                var pressureSensor = (IPressureSensor)sensorService.GetSensor(pressureSensorResult.Id);
                exposure.StartPressure = pressureSensorResult.Value.ToUnit(bench.Settings.PressureUnit).ToString(pressureSensor.Accaury);
                exposure.StopPressure = registration.StopPressure.FirstOrDefault().Value.ToUnit(bench.Settings.PressureUnit).ToString(pressureSensor.Accaury);
                exposure.Result = registration.Result + " " + testResult.Standart;
                Results.Add(exposure);

            }
            Add = ReactiveCommand.Create(() => dialogService.CloseAsync(true));
            
        }

        public List<ExposureViewModel> Results { get; set; } = new List<ExposureViewModel>();
        

    }
}
