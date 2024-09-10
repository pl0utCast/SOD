using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor
{
    public interface IDeviceSensorViewModel
    {
        public string Name { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
    }
}
