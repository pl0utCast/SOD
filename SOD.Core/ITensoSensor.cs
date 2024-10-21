using SOD.Core.Sensor;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core
{
	public interface ITensoSensor : ISensor, IObservable<Mass>
	{
		public Mass Mass { get; }
		public Mass MaxValue { get; }
		public Mass MinValue { get; }
	}
}
