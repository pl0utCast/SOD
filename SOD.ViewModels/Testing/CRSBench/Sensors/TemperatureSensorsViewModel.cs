using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Benches.CRSBench;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SOD.ViewModels.Testing.CRSBench.Sensors
{
    public class TemperatureSensorsViewModel : ReactiveObject, IActivatableViewModel
    {
        public TemperatureSensorsViewModel(ISensorService sensorService, Bench bench)
        {
            this.WhenActivated(dis =>
            {
                IsEnable = false;
                LiquidTemp = null;
                GasTemp = null;
                if (bench.Settings.IsEnableGasTemperatureSensor)
                {
                    IsEnable = true;
                    var gasSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.GasTemperatureSensorId);
                    if (gasSensor!=null)
                    {
                        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                              .Subscribe(t =>
                              {
                                  GasTemp = ((ITemperatureSensor)gasSensor).Temperature.ToString(gasSensor.Accaury);
                              })
                              .DisposeWith(dis);
                    }
                    
                }
                if (bench.Settings.IsEnableLiquidTemperatureSensor)
                {
                    IsEnable = true;
                    var liquidSensor = sensorService.GetAllSensors().SingleOrDefault(s => s.Id == bench.Settings.LiquidTemperatureSensorId);
                    if (liquidSensor!=null)
                    {
                        Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                              .Subscribe(t =>
                              {
                                  LiquidTemp = ((ITemperatureSensor)liquidSensor).Temperature.ToString(liquidSensor.Accaury);
                              })
                              .DisposeWith(dis);
                    }
                    
                }
            });
        }
        [Reactive]
        public bool IsEnable { get; set; }
        [Reactive]
        public string LiquidTemp { get; set; }
        [Reactive]
        public string GasTemp { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
