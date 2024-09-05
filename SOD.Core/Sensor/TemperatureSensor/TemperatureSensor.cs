using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor.TemperatureSensor
{
    public class TemperatureSensor : ITemperatureSensor
    {
        private readonly string settingsKey = "TemperatureSettings_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private readonly IChannelBasedDevice channelBasedDevice;
        private readonly ISettingsService _settingsService;
        private IDisposable disposable;
        public TemperatureSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            this.channelBasedDevice = channelBasedDevice;
            _settingsService = settingsService;
            Settings = settingsService.GetSettings(settingsKey + id, new TemperartureSensorSettings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            Settings.Id = id;
            Connect();
        }
        public Temperature Temperature { get; private set; }

        public int Id => Settings.Id;


        public TemperartureSensorSettings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public string Name => Settings.Name;

        public string SensorHint => Settings.SensorHint;

        public string Accaury => Settings.Accaury;

        public void Connect()
        {
            disposable = channelBasedDevice.DataComplite.Subscribe(c =>
            {
                if (c.Id == Settings.ChannelId && c.DataType == ChannelDataType.DOUBLE)
                {
                    Temperature = new Temperature((double)c.Value, Settings.Unit);
                }
            });
        }

        public void Disconnect()
        {
            disposable?.Dispose();
        }

        public void SaveSettings()
        {
            _settingsService.SaveSensorSettings(settingsKey + Id, Settings);

            // Записываем текущее время, чтобы можно было узнать когда последний раз калибровался файл
            LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
            _settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
        }
    }
}
