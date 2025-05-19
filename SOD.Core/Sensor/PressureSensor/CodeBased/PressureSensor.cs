using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor.PressureSensor.CodeBased
{
    public class PressureSensor : BaseSensor<Pressure>, IPressureSensor, ICodeBasedSensor
    {
        private readonly IChannelBasedDevice channelBasedDevice;
        private readonly ISettingsService _settingsService;
        private const string SETTINGS_KEY = "CodeBasedPressureSensor_Id_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private double code;
        private IDisposable disposable;
        private DigitalFilter filter;
        //private double oldValue;

        public PressureSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            Id = id;
            this.channelBasedDevice = channelBasedDevice;
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(SETTINGS_KEY + id, new PressureSensorSettings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
            Connect();
        }
        public Pressure Pressure { get; private set; }

        public int Id { get; private set; }

        public string Name => Settings.Name;

        public double Code => code;

        public void SaveSettings()
        {
            _settingsService.SaveSensorSettings(SETTINGS_KEY+Id, Settings);

            // Записываем текущее время, чтобы можно было узнать когда последний раз калибровался файл
            LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
            _settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
        }

        public void Connect()
        {
            disposable = channelBasedDevice.DataComplite.Subscribe(dc =>
            {
                if (dc.Id == Settings.ChannelId && (dc.DataType == ChannelDataType.INT16 
                                                 || dc.DataType == ChannelDataType.INT
                                                 || dc.DataType == ChannelDataType.FLOAT
                                                 || dc.DataType == ChannelDataType.DOUBLE))
                {
                    //Random rnd = new Random();
                    //code = rnd.Next(1560, 1620);
                    code = Math.Round(Convert.ToDouble(dc.Value), 3);
                    var rawValue = this.CodeToValue(Settings.MinValue.Bars, Settings.MaxValue.Bars, Settings.MinCode, Settings.MaxCode, code);
                    rawValue = filter.Filtering(rawValue); // Фильтруем значение

                    var k = Settings.MaxValue.Bars * 0.2 / 100;
                    var n = Settings.MaxValue.Bars * -0.2 / 100;

                    if (rawValue < k && rawValue > n)
                    {
                        Pressure = new Pressure(0, UnitsNet.Units.PressureUnit.Bar).ToUnit(Settings.Unit);
                    }
                    else
                    {
                        Pressure = new Pressure(rawValue, UnitsNet.Units.PressureUnit.Bar).ToUnit(Settings.Unit);
                    }

                    Notify(Pressure);
                }
            });
        }

        public void Disconnect()
        {
            disposable?.Dispose();
        }

        public PressureSensorSettings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public Pressure MaxValue => Settings.MaxValue;

        public Pressure MinValue => Settings.MinValue;

        public string Accaury => Settings.Accaury;

        public string SensorHint => Settings.SensorHint;
    }
}
