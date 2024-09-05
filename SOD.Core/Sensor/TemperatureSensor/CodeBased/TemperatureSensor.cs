using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor.TemperatureSensor.CodeBased
{
    public class TemperatureSensor : BaseSensor<Temperature>, ITemperatureSensor, ICodeBasedSensor
    {
        private const string SETTINGS_KEY = "CodeBasedTemperatureSensor_Id_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private readonly IChannelBasedDevice channelBasedDevice;
        private readonly ISettingsService _settingsService;
        private IDisposable disposable;
        private DigitalFilter filter;

        public TemperatureSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            Id = id;
            this.channelBasedDevice = channelBasedDevice;
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(SETTINGS_KEY + id, new Settings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
            Temperature = new Temperature(0, UnitsNet.Units.TemperatureUnit.DegreeCelsius).ToUnit(Settings.Unit);
            Connect();
        }
        public void SaveSettings()
        {
            _settingsService.SaveSensorSettings(SETTINGS_KEY + Id, Settings);

            // Записываем текущее время, чтобы можно было узнать когда последний раз калибровался файл
            LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
            _settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
        }

        public void Connect()
        {
            disposable = channelBasedDevice.DataComplite.Subscribe(dc =>
            {
                if (dc.Id == Settings.ChannelId && dc.DataType == ChannelDataType.INT)
                {
                    Code = Convert.ToInt32(dc.Value);
                    var rawValue = this.CodeToValue(Settings.MinValue.DegreesCelsius, Settings.MaxValue.DegreesCelsius, Settings.MinCode, Settings.MaxCode, Code);
                    
                    var k = Settings.MaxValue.DegreesCelsius * 0.2 / 100;
                    var n = Settings.MaxValue.DegreesCelsius * -0.2 / 100;

                    if (rawValue < k && rawValue > n)
                    {
                        Temperature = new Temperature(0, UnitsNet.Units.TemperatureUnit.DegreeCelsius).ToUnit(Settings.Unit);
                    }
                    else
                    {
                        Temperature = new Temperature(filter.Filtering(rawValue), UnitsNet.Units.TemperatureUnit.DegreeCelsius).ToUnit(Settings.Unit);
                    }

                    Notify(Temperature);
                }

            });
        }

        public void Disconnect()
        {
            disposable?.Dispose();
        }

        public Temperature Temperature { get; private set; }

        public int Id { get; private set; }

        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public int Code { get; private set; }
        public Settings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public string Accaury => Settings.Accaury;
    }
}
