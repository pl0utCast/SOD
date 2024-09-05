using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor.PressureSensor
{
    public class PressureSensor : BaseSensor<Pressure>, IPressureSensor
    {
        private readonly IChannelBasedDevice channelBasedDevice;
        private readonly ISettingsService _settingsService;
        private string settingsKey = "PessureSensor_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private IDisposable valueUpdateDisposable;
        public PressureSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            this.channelBasedDevice = channelBasedDevice;
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(settingsKey + id, new PressureSensorSettings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            Settings.Id = id;
            Connect();
            
        }
        public Pressure Pressure { get; private set; }

        public int Id => Settings.Id;
        public PressureSensorSettings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public Pressure MaxValue => throw new NotImplementedException();

        public Pressure MinValue  => throw new NotImplementedException();

        public string Accaury => Settings.Accaury;

        public void Connect()
        {
            valueUpdateDisposable = channelBasedDevice.DataComplite.Subscribe(channel =>
            {
                if (channel.Id != Settings.ChannelId || channel.DataType != ChannelDataType.DOUBLE) return;
                Pressure = new Pressure((double)channel.Value, Settings.PressureUnit);
                Notify(Pressure);
            });
        }

        public void Disconnect()
        {
            valueUpdateDisposable?.Dispose();
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
