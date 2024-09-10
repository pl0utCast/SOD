using SOD.Core.Units;
using SOD.ViewModels.Controls;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Sensors.Dialog
{
    public class CodeBasedSensorSettingsViewModel : YesNoDialogViewModel, IActivatableViewModel
    {
        public CodeBasedSensorSettingsViewModel(IDialogService dialogService, Func<string> valueUpdater, Func<int> codeUpdater) : base(dialogService)
        {
            SetMinCode = ReactiveCommand.Create(() =>
            {
                MinCode = Code;
            });
            SetMaxCode = ReactiveCommand.Create(() =>
            {
                MaxCode = Code;
            });
            this.WhenActivated(dis =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                          .Subscribe(t =>
                          {
                              Value = valueUpdater();
                              Code = codeUpdater();
                          })
                          .DisposeWith(dis);
                
            });
        }
        public int Id { get; set; }
        public int ChannelId { get; set; }
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public int MinCode { get; set; }
        [Reactive]
        public int MaxCode { get; set; }
        [Reactive]
        public int Code { get; set; }
        [Reactive]
        public string Value { get; set; }
        public UnitValueViewModel MinValue { get; set; }
        public UnitValueViewModel MaxValue { get; set; }
        public string Accaury { get; set; }
        public double FilterCoef { get; set; }
        public string SensorHint { get; set; }
        public IEnumerable<UnitTypeInfo> UnitTypes { get; set; }
        [Reactive]
        public UnitTypeInfo UnitType { get; set; }
        public ReactiveCommand<Unit, Unit> SetMinCode { get; set; }
        public ReactiveCommand<Unit, Unit> SetMaxCode { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
