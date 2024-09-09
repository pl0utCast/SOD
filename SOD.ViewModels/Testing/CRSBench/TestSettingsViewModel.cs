using SOD.App.Testing;
using SOD.App.Testing.Standarts;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using SOD.Core.Sensor;
using SOD.App.Mediums;
using ReactiveUI.Validation.Helpers;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class TestSettingsViewModel : ReactiveValidationObject, IActivatableViewModel
    {
        public TestSettingsViewModel(App.Benches.CRSBench.Settings.TestSettings testSettings, IEnumerable<IStandart> standarts, IEnumerable<ISensor> sensors)
        {
            Settings = testSettings;
            Standarts = standarts;
            Standart = standarts.SingleOrDefault(s => s.Id == testSettings.StandartId);
            PressureSensors = sensors.Where(s => s is IPressureSensor);
            PressureSensor = PressureSensors.SingleOrDefault(s => s.Id == Settings.PressureSensorId);
            Medium = Settings.MediumType;

            this.WhenActivated(dis =>
            {
                this.ValidationRule(x => x.PressureSensor, s => s != null, "Error").DisposeWith(dis);
            });
        }

        public virtual void Save()
        {
            Settings.Time = Time;
            Settings.StandartId = Standart?.Id;
            Settings.PressureSensorId = PressureSensor.Id;
            Settings.MediumType = Medium;
        }

        [Reactive]
        public MediumType Medium { get; set; }
        public string Name { get; set; }
        public TestType Type { get; set; }
        [Reactive]
        public int? Time { get; set; }
        [Reactive]
        public IStandart Standart { get; set; }
        public IEnumerable<IStandart> Standarts { get; set; }
        public IEnumerable<ISensor> PressureSensors { get; set; }
        [Reactive]
        public ISensor PressureSensor { get; set; }
        public App.Benches.CRSBench.Settings.TestSettings Settings { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        //???????
        //public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}
