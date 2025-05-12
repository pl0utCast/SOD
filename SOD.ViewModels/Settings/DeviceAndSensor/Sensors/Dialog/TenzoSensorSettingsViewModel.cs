using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Sensor.TenzoSensor.CodeBased;
using SOD.Core.Units;
using SOD.Dialogs;
using SOD.ViewModels.Controls;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UnitsNet;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Sensors.Dialog
{
    public class TenzoSensorSettingsViewModel : YesNoDialogViewModel, IActivatableViewModel
    {
        public TenzoSensorSettingsViewModel(IDialogService dialogService, object codeBasedTenzoSensor, Func<string> valueUpdater, Func<int> codeUpdater) : base(dialogService)
        {
            CoefCalibr = ReactiveCommand.Create(() =>
            {
                dialogService.ShowDialog("CoefficentCalibration", new CoefficentCalibrationViewModel(dialogService, (TenzoSensor)codeBasedTenzoSensor));
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
        public int Code { get; set; }
        [Reactive]
        public string Value { get; set; }
        [Reactive]
        public UnitValueViewModel MaxValue { get; set; }
        public UnitValueViewModel MinValue { get; set; }
        public string Accaury { get; set; }
        public string SensorHint { get; set; }
        public IEnumerable<UnitTypeInfo> UnitTypes { get; set; }
        [Reactive]
        public UnitTypeInfo UnitType { get; set; }
        public ReactiveCommand<Unit, Unit> CoefCalibr { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
