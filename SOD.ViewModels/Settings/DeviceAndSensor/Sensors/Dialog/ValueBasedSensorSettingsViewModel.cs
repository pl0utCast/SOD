using SOD.Core.Units;
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
    public class ValueBasedSensorSettingsViewModel : ReactiveObject, IActivatableViewModel
    {
        public ValueBasedSensorSettingsViewModel(Func<string> valueUpdater, IDialogService dialogService)
        {
            this.WhenActivated(disposables =>
            {
                Observable
                .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    CurrentValue = valueUpdater.Invoke();
                })
                .DisposeWith(disposables);
            });

            Save = ReactiveCommand.Create(() =>
            {
                dialogService.CloseAsync(true);
            }, this.WhenAny(x=>x.SelectedUnit, s=>s.Value!=null));
            Cancel = ReactiveCommand.Create(() =>
            {
                dialogService.CloseAsync(false);
            });
        }
        public int Id { get; set; }
        [Reactive]
        public int ChannelId { get; set; }
        [Reactive]
        public string CurrentValue { get; set; }
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Accaury { get; set; }
        [Reactive]
        public double FilterCoef { get; set; }
        [Reactive]
        public string SensorHint { get; set; }
        [Reactive]
        public UnitTypeInfo SelectedUnit { get; set; }
        public IReadOnlyList<UnitTypeInfo> UnitTypes { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
