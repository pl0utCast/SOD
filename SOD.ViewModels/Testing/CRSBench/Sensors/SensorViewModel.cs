using SOD.Core.Sensor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.CRSBench.Sensors
{
    public class SensorViewModel : ReactiveObject, IActivatableViewModel
    {
        public SensorViewModel(ISensor sensor)
        {
            Sensor = sensor;  
        }
        [Reactive]
        public string Value { get; set; }
        [Reactive]
        public string Unit { get; set; }
        [Reactive]
        public string Name { get; set; }
        public ISensor Sensor { get; set; }


        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
