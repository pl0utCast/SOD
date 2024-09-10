using SOD.Core.Sensor;
using SOD.Core.Sensor.Frenq;
using SOD.Core.Sensor.LeakageSensor.Impulse;
using SOD.Core.Sensor.PressureSensor;
using SOD.Core.Sensor.TemperatureSensor;
using SOD.Core.Units;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;
using System.Reactive;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Sensors
{
    public class SensorViewModel : ReactiveObject, IDeviceSensorViewModel
    {
        public SensorViewModel(ISensor sensor, IDialogService dialogService)
        {
            Name = sensor.Name;
            SensorHint = sensor.SensorHint;
            Edit = ReactiveCommand.CreateFromTask(async () =>
            {
                if (sensor is PressureSensor pressureSensor)
                {
                    var vm = new Dialog.ValueBasedSensorSettingsViewModel(() => pressureSensor.Pressure.ToString(pressureSensor.Settings.Accaury), dialogService);
                    vm.Id = sensor.Id;
                    vm.Name = sensor.Name;
                    vm.Accaury = pressureSensor.Settings.Accaury;
                    vm.FilterCoef = pressureSensor.Settings.FilterCoef;
                    vm.ChannelId = pressureSensor.Settings.ChannelId;
                    vm.UnitTypes = pressureSensor.Pressure.GetUnitTypeInfo();
                    vm.SelectedUnit = vm.UnitTypes.SingleOrDefault(ui => ui.UnitType.ToString() == pressureSensor.Settings.PressureUnit.ToString());
                    vm.SensorHint = pressureSensor.Settings.SensorHint;

                    if ((bool)await dialogService.ShowDialogAsync("ValueBasedSensorSettings", vm))
                    {
                        pressureSensor.Settings.Name = vm.Name;
                        pressureSensor.Settings.Accaury = vm.Accaury;
                        pressureSensor.Settings.FilterCoef = vm.FilterCoef;
                        pressureSensor.Settings.PressureUnit = (PressureUnit)vm.SelectedUnit.UnitType;
                        pressureSensor.Settings.ChannelId = vm.ChannelId;
                        pressureSensor.Settings.SensorHint = vm.SensorHint;
                        pressureSensor.SaveSettings();
                    }
                }
                else if (sensor is TemperatureSensor temperatureSensor)
                {
                    var vm = new Dialog.ValueBasedSensorSettingsViewModel(() => temperatureSensor.Temperature.ToString(temperatureSensor.Settings.Accaury), dialogService);
                    vm.Id = sensor.Id;
                    vm.Name = sensor.Name;
                    vm.ChannelId = temperatureSensor.Settings.ChannelId;
                    vm.Accaury = temperatureSensor.Settings.Accaury;
                    vm.FilterCoef = temperatureSensor.Settings.FilterCoef;
                    vm.UnitTypes = temperatureSensor.Temperature.GetUnitTypeInfo();
                    vm.SelectedUnit = vm.UnitTypes.SingleOrDefault(ui => ui.UnitType.ToString() == temperatureSensor.Settings.Unit.ToString());
                    vm.SensorHint = temperatureSensor.Settings.SensorHint;

                    if ((bool)await dialogService.ShowDialogAsync("ValueBasedSensorSettings", vm))
                    {
                        temperatureSensor.Settings.Name = vm.Name;
                        temperatureSensor.Settings.Accaury = vm.Accaury;
                        temperatureSensor.Settings.FilterCoef = vm.FilterCoef;
                        temperatureSensor.Settings.Unit = (TemperatureUnit)vm.SelectedUnit.UnitType;
                        temperatureSensor.Settings.ChannelId = vm.ChannelId;
                        temperatureSensor.Settings.SensorHint = vm.SensorHint;
                        temperatureSensor.SaveSettings();
                    }
                }
                else if (sensor is Core.Sensor.PressureSensor.CodeBased.PressureSensor codeBasedPressureSensor)
                {
                    var vm = new Dialog.CodeBasedSensorSettingsViewModel(dialogService, () => codeBasedPressureSensor.Pressure.ToString(codeBasedPressureSensor.Settings.Accaury), () => codeBasedPressureSensor.Code);
                    vm.Id = codeBasedPressureSensor.Id;
                    vm.Name = codeBasedPressureSensor.Name;
                    vm.MinCode = codeBasedPressureSensor.Settings.MinCode;
                    vm.MaxCode = codeBasedPressureSensor.Settings.MaxCode;
                    vm.MaxValue = new Controls.UnitValueViewModel(codeBasedPressureSensor.Settings.MaxValue);
                    vm.MinValue = new Controls.UnitValueViewModel(codeBasedPressureSensor.Settings.MinValue);
                    vm.Accaury = codeBasedPressureSensor.Settings.Accaury;
                    vm.FilterCoef = codeBasedPressureSensor.Settings.FilterCoef;
                    vm.ChannelId = codeBasedPressureSensor.Settings.ChannelId;
                    vm.UnitTypes = new Pressure().GetUnitTypeInfo();
                    vm.UnitType = vm.UnitTypes.SingleOrDefault(ut => ut.UnitType.ToString() == codeBasedPressureSensor.Settings.Unit.ToString());
                    vm.SensorHint = codeBasedPressureSensor.Settings.SensorHint;

                    if ((bool)await dialogService.ShowDialogAsync("CodeBasedSensorSettings", vm))
                    {
                        codeBasedPressureSensor.Settings.Name = vm.Name;
                        codeBasedPressureSensor.Settings.ChannelId = vm.ChannelId;
                        codeBasedPressureSensor.Settings.MinCode = vm.MinCode;
                        codeBasedPressureSensor.Settings.MaxCode = vm.MaxCode;
                        codeBasedPressureSensor.Settings.MinValue = (Pressure)vm.MinValue.GetValue();
                        codeBasedPressureSensor.Settings.MaxValue = (Pressure)vm.MaxValue.GetValue();
                        codeBasedPressureSensor.Settings.Accaury = vm.Accaury;
                        codeBasedPressureSensor.Settings.FilterCoef = vm.FilterCoef;
                        codeBasedPressureSensor.Settings.Unit = (PressureUnit)vm.UnitType?.UnitType;
                        codeBasedPressureSensor.Settings.SensorHint = vm.SensorHint;
                        codeBasedPressureSensor.SaveSettings();
                    }
                }
                else if (sensor is Core.Sensor.LeakageSensor.CodeBased.LeakageSensor codeBasedLeakageSensor)
                {
                    var vm = new Dialog.CodeBasedSensorSettingsViewModel(dialogService, () => codeBasedLeakageSensor.Flow.ToString(codeBasedLeakageSensor.Accaury), () => codeBasedLeakageSensor.Code);
                    vm.Id = codeBasedLeakageSensor.Id;
                    vm.Name = codeBasedLeakageSensor.Name;
                    vm.MinCode = codeBasedLeakageSensor.Settings.MinCode;
                    vm.MaxCode = codeBasedLeakageSensor.Settings.MaxCode;
                    vm.MaxValue = new Controls.UnitValueViewModel(codeBasedLeakageSensor.Settings.MaxValue);
                    vm.MinValue = new Controls.UnitValueViewModel(codeBasedLeakageSensor.Settings.MinValue);
                    vm.Accaury = codeBasedLeakageSensor.Settings.Accaury;
                    vm.FilterCoef = codeBasedLeakageSensor.Settings.FilterCoef;
                    vm.ChannelId = codeBasedLeakageSensor.Settings.ChannelId;
                    vm.UnitTypes = new VolumeFlow().GetUnitTypeInfo();
                    vm.UnitType = vm.UnitTypes.SingleOrDefault(ut => ut.UnitType.ToString() == codeBasedLeakageSensor.Settings.Unit.ToString());
                    vm.SensorHint = codeBasedLeakageSensor.Settings.SensorHint;

                    if ((bool)await dialogService.ShowDialogAsync("CodeBasedSensorSettings", vm))
                    {
                        codeBasedLeakageSensor.Settings.Name = vm.Name;
                        codeBasedLeakageSensor.Settings.ChannelId = vm.ChannelId;
                        codeBasedLeakageSensor.Settings.MinCode = vm.MinCode;
                        codeBasedLeakageSensor.Settings.MaxCode = vm.MaxCode;
                        codeBasedLeakageSensor.Settings.MinValue = (VolumeFlow)vm.MinValue.GetValue();
                        codeBasedLeakageSensor.Settings.MaxValue = (VolumeFlow)vm.MaxValue.GetValue();
                        codeBasedLeakageSensor.Settings.Accaury = vm.Accaury;
                        codeBasedLeakageSensor.Settings.FilterCoef = vm.FilterCoef;
                        codeBasedLeakageSensor.Settings.Unit = (VolumeFlowUnit)vm.UnitType?.UnitType;
                        codeBasedLeakageSensor.Settings.SensorHint = vm.SensorHint;
                        codeBasedLeakageSensor.SaveSettings();
                    }
                }
                else if (sensor is Core.Sensor.TemperatureSensor.CodeBased.TemperatureSensor codeBasedTemperatureSensor)
                {
                    var vm = new Dialog.CodeBasedSensorSettingsViewModel(dialogService, () => codeBasedTemperatureSensor.Temperature.ToString(codeBasedTemperatureSensor.Settings.Accaury), () => codeBasedTemperatureSensor.Code);
                    vm.Id = codeBasedTemperatureSensor.Id;
                    vm.Name = codeBasedTemperatureSensor.Name;
                    vm.MinCode = codeBasedTemperatureSensor.Settings.MinCode;
                    vm.MaxCode = codeBasedTemperatureSensor.Settings.MaxCode;
                    vm.MaxValue = new Controls.UnitValueViewModel(codeBasedTemperatureSensor.Settings.MaxValue);
                    vm.MinValue = new Controls.UnitValueViewModel(codeBasedTemperatureSensor.Settings.MinValue);
                    vm.Accaury = codeBasedTemperatureSensor.Settings.Accaury;
                    vm.FilterCoef = codeBasedTemperatureSensor.Settings.FilterCoef;
                    vm.ChannelId = codeBasedTemperatureSensor.Settings.ChannelId;
                    vm.UnitTypes = new Temperature().GetUnitTypeInfo();
                    vm.UnitType = vm.UnitTypes.SingleOrDefault(ut => ut.UnitType.ToString() == codeBasedTemperatureSensor.Settings.Unit.ToString());
                    vm.SensorHint = codeBasedTemperatureSensor.Settings.SensorHint;

                    if ((bool)await dialogService.ShowDialogAsync("CodeBasedSensorSettings", vm))
                    {
                        codeBasedTemperatureSensor.Settings.Name = vm.Name;
                        codeBasedTemperatureSensor.Settings.ChannelId = vm.ChannelId;
                        codeBasedTemperatureSensor.Settings.MinCode = vm.MinCode;
                        codeBasedTemperatureSensor.Settings.MaxCode = vm.MaxCode;
                        codeBasedTemperatureSensor.Settings.Accaury = vm.Accaury;
                        codeBasedTemperatureSensor.Settings.FilterCoef = vm.FilterCoef;
                        codeBasedTemperatureSensor.Settings.MinValue = (Temperature)vm.MinValue.GetValue();
                        codeBasedTemperatureSensor.Settings.MaxValue = (Temperature)vm.MaxValue.GetValue();
                        codeBasedTemperatureSensor.Settings.Unit = (TemperatureUnit)vm.UnitType?.UnitType;
                        codeBasedTemperatureSensor.Settings.SensorHint = vm.SensorHint;
                        codeBasedTemperatureSensor.SaveSettings();
                    }
                }
            });
        }
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string SensorHint { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
    }
}
