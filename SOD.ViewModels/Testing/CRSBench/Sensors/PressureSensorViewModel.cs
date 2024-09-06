using ReactiveUI;
using SOD.Core.Sensor;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Testing.CRSBench.Sensors
{
    public class PressureSensorViewModel : SensorViewModel
    {
        public PressureSensorViewModel(IPressureSensor sensor, PressureUnit pressureUnit) : base(sensor)
        {
            this.WhenActivated(dis =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                          .Subscribe(t =>
                          {
                              Value = sensor.Pressure.As(pressureUnit).ToString(sensor.Accaury);
                          })
                          .DisposeWith(dis);
            });
            Unit = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(typeof(PressureUnit), (int)pressureUnit);
            Name = sensor.Name;
        }
    }
}
