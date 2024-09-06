using SOD.Core.Sensor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Testing.CRSBench.Sensors
{
    public class LeakageSensorViewModel : SensorViewModel
    {
        public LeakageSensorViewModel(ILeakageSensor leakageSensor, VolumeFlowUnit volumeFlowUnit) : base(leakageSensor)
        {
            this.WhenActivated(dis =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                          .Subscribe(t =>
                          {
                              //закомитить 3 нижние строки для чехов
                              //if (volumeFlowUnit == VolumeFlowUnit.CubicCentimeterPerMinute)
                              //    Value = leakageSensor.Flow.As(volumeFlowUnit).ToString("F3");
                              //else
                                  Value = leakageSensor.Flow.As(volumeFlowUnit).ToString(leakageSensor.Accaury);
                          })
                          .DisposeWith(dis);
            });

            Unit = Unit = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(typeof(VolumeFlowUnit), (int)volumeFlowUnit);
            Name = leakageSensor.Name;
        }
        
    }
}
