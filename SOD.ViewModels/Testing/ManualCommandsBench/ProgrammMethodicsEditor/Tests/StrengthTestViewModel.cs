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
    public class StrengthTestViewModel : ReactiveObject, ISupportSave, IActivatableViewModel
    {
        private TestProgrammConfig _testProgrammConfig;
        public StrengthTestViewModel(TestProgrammConfig testProgrammConfig)
        {
            _testProgrammConfig = testProgrammConfig;
            Pressure = new UnitValueViewModel(new Pressure(0, UnitsNet.Units.PressureUnit.Bar));
        }

        public void Save()
        {
            _testProgrammConfig.Parameters.Clear();
            _testProgrammConfig.Parameters.Add(new KeyValuePair<Type, object>(typeof(Pressure), Pressure.GetValue()));
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            throw new NotImplementedException();
        }

        public UnitValueViewModel Pressure { get; set; }
        [Reactive]
        public bool IsManual { get; set; }

        public bool CanSave => throw new NotImplementedException();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
