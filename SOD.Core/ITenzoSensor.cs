using UnitsNet;

namespace SOD.Core.Sensor
{
	public interface ITenzoSensor : ISensor, IObservable<Force>
	{
        public Force Force { get; }
    }
}
