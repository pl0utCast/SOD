using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnitsNet;

namespace SOD.Core.Sensor.LeakageSensor.CodeBased
{
    public class LeakageSensor : BaseSensor<VolumeFlow>, ILeakageSensor, ICodeBasedSensor
    {
        private const string SETTINGS_KEY = "CodeBasedLeakageSensor_Id_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private readonly IChannelBasedDevice channelBasedDevice;
        private readonly ISettingsService _settingsService;
        private int code;
        private TimeSpan time;
        private Stopwatch stopwatch = new Stopwatch();
        private Object locker = new object();
        private IDisposable disposable;
        private DigitalFilter filter;

        public LeakageSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            Id = id;
            this.channelBasedDevice = channelBasedDevice;
            _settingsService = settingsService;
            Settings = settingsService.GetSettings(SETTINGS_KEY + Id, new Settings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            stopwatch.Start();
            filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
            Connect();
        }
        

        public void Reset()
        {
            lock(locker)
            {
                time = new TimeSpan();
            }
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
                if (dc.Id == Settings.ChannelId && (dc.DataType == ChannelDataType.INT || dc.DataType == ChannelDataType.INT16))
                {
                    time += stopwatch.Elapsed;
                    code = Convert.ToInt32(dc.Value);
                    var rawValue = this.CodeToValue(Settings.MinValue.CubicCentimetersPerMinute, Settings.MaxValue.CubicCentimetersPerMinute, Settings.MinCode, Settings.MaxCode, code);

                    var k = Settings.MaxValue.CubicCentimetersPerMinute * 2 / 100;
                    var n = Settings.MaxValue.CubicCentimetersPerMinute * -2 / 100;

                    if (rawValue < k  && rawValue > n)
                    {
                        Flow = new VolumeFlow(0, UnitsNet.Units.VolumeFlowUnit.CubicCentimeterPerMinute).ToUnit(Settings.Unit);
                    }
                    else
                    {
                        Flow = new VolumeFlow(filter.Filtering(rawValue), UnitsNet.Units.VolumeFlowUnit.CubicCentimeterPerMinute).ToUnit(Settings.Unit);
                    }

                    Volume = Flow * time;
                    Notify(Flow);
                }
            });
        }

        public void Disconnect()
        {
            disposable?.Dispose();
        }

        public VolumeFlow Flow { get; private set; }

        public Volume Volume { get; private set; }

        public int Id { get; private set; }

        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public int Code => code;
        public Settings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public VolumeFlow MaxValue => Settings.MaxValue;

        public string Accaury => Settings.Accaury;
    }
}
