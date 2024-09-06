using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using SOD.App.Testing.Standarts;
using SOD.Core.Sensor;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class LeakageTestSettingsViewModel : TestSettingsViewModel
    {
        public LeakageTestSettingsViewModel(App.Benches.CRSBench.Settings.TestSettings testSettings,
                                            IEnumerable<IStandart> standarts, IEnumerable<ISensor> sensors) : base(testSettings, standarts, sensors)
        {
            LeakageSensors = sensors.Where(s => s is ILeakageSensor);
            LeakageSensor = LeakageSensors.SingleOrDefault(s => s.Id == Settings.LeakageSensorId);

            this.WhenActivated(dis =>
            {
                this.ValidationRule(x => x.LeakageSensor, s => s != null, "Error").DisposeWith(dis);
                this.ValidationRule(x => x.Time, t => t != null && t != 0, "Error").DisposeWith(dis);
                this.ValidationRule(x => x.Standart, s => s != null, "Error").DisposeWith(dis);
            });
        }
        public override void Save()
        {
            base.Save();
            Settings.LeakageSensorId = LeakageSensor?.Id;
        }
        public IEnumerable<ISensor> LeakageSensors { get; set; }
        [Reactive]
        public ISensor LeakageSensor { get; set; }
    }
}
