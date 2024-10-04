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

namespace SOD.ViewModels.Testing.CRSBench.Sensors
{
	public class TensoSensorViewModel : SensorViewModel
	{
		public TensoSensorViewModel(ITensoSensor sensor, MassUnit tensoUnit) : base(sensor)
		{
			this.WhenActivated(dis =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
						  .Subscribe(t =>
						  {
							  Value = sensor.Mass.As(tensoUnit).ToString(sensor.Accaury);
						  })
						  .DisposeWith(dis);
			});
			Unit = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(typeof(MassUnit), (int)tensoUnit);
			Name = sensor.Name;
		}
	}
}
