using ReactiveUI;
using SOD.Core;
using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet.Units;
using UnitsNet;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using SOD.LocalizationService;
using System.Globalization;

namespace SOD.ViewModels.Testing.SODBench.Sensors
{
	public class TenzoSensorViewModel : SensorViewModel
	{
		public TenzoSensorViewModel(ITenzoSensor sensor, ForceUnit tenzoUnit, ILocalizationService localizationService) : base(sensor)
		{
			this.WhenActivated(dis =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
						  .Subscribe(t =>
						  {
							  Value = sensor.Force.As(tenzoUnit).ToString(sensor.Accaury);
                          })
                .DisposeWith(dis);
            });

            Unit = Force.GetAbbreviation(tenzoUnit, new CultureInfo(localizationService.CurrentCulture.Name));
            Name = sensor.Name;
		}
	}
}
