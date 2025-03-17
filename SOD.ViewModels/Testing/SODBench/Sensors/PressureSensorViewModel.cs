using ReactiveUI;
using SOD.Core.Sensor;
using SOD.LocalizationService;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Testing.SODBench.Sensors
{
    public class PressureSensorViewModel : SensorViewModel
    {
        public PressureSensorViewModel(IPressureSensor sensor, PressureUnit pressureUnit, ILocalizationService localizationService) : base(sensor)
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

            Unit = Pressure.GetAbbreviation(pressureUnit, new CultureInfo(localizationService.CurrentCulture.Name));
            Name = sensor.Name;
        }
    }
}
