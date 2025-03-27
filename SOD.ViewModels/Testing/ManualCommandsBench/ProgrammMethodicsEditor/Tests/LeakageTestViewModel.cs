using SOD.App.Testing.Programms;
using SOD.ViewModels.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using UnitsNet;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Tests
{
    public class LeakageTestViewModel : ReactiveObject, ISupportSave, IActivatableViewModel
    {
        private readonly TestProgrammConfig _testProgrammConfig;
        public LeakageTestViewModel(TestProgrammConfig testProgrammConfig)
        {

            _testProgrammConfig = testProgrammConfig;
            Leakage = new UnitValueViewModel(new VolumeFlow(0, UnitsNet.Units.VolumeFlowUnit.CubicMeterPerMinute));
            if (testProgrammConfig.Parameters.Count > 0)
            {

            }
        }

        public void Save()
        {
            _testProgrammConfig.Parameters.Clear();
            _testProgrammConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(VolumeFlow), Leakage.GetValue()));
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            throw new NotImplementedException();
        }
        public UnitValueViewModel Leakage { get; set; }
        [Reactive]
        public bool IsManual { get; set; }
        public bool CanSave => throw new NotImplementedException();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
