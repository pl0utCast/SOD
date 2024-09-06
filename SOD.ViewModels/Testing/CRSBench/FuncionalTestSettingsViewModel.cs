using MemBus;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor;
using SOD.Core.Valves;
using SOD.App.Benches;
using SOD.App.Benches.CRSBench;
using SOD.App.Testing.Standarts;
using SOD.ViewModels.Controls;
using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class FuncionalTestSettingsViewModel : TestSettingsViewModel
    {
        private readonly App.Benches.CRSBench.Settings.TestSettings testSettings;

        public FuncionalTestSettingsViewModel(App.Benches.CRSBench.Settings.TestSettings testSettings,
                                            IEnumerable<IStandart> standarts, IEnumerable<ISensor> sensors) : base(testSettings, standarts, sensors)
        {
            SetPressure = new UnitValueViewModel(testSettings.SetPressure);
            UseValveSetPressure = testSettings.UseValveSetPressure;
            this.testSettings = testSettings;
            
        }

        public override void Save()
        {
            base.Save();
            testSettings.SetPressure = (Pressure)SetPressure.GetValue();
            testSettings.UseValveSetPressure = UseValveSetPressure;
        }
        [Reactive]
        public UnitValueViewModel SetPressure { get; set; }
        [Reactive]
        public bool UseValveSetPressure { get; set; }
    }
}
