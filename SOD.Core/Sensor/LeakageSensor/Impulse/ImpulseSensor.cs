using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.LeakageSensor.Impulse
{
    public class ImpulseSensor : BaseSensor<VolumeFlow>, ILeakageSensor, IDisposable
    {
        private IChannelBasedDevice ownerDevice;
        private readonly ISettingsService _settingsService;

        private string settingsKey = "ImpulseSensor_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private IDisposable ownerDeviceDataCompliteDisposable;
        private Stopwatch stopwatch = new Stopwatch();
        private uint oldCounter = 0;
        private uint counter = 0;
        private DigitalFilter filter;
        public ImpulseSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(settingsKey + id, new ImpulseSensorSettings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            Settings.Id = id;
            ownerDevice = channelBasedDevice;
            filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
            Connect();
        }

        public VolumeFlow Flow { get; private set; } = new VolumeFlow();

        public Volume Volume => new Volume(Counter * Settings.ImpulsePrice.CubicCentimeters, VolumeUnit.CubicCentimeter);
        public uint Counter { get; private set; }

        public int Id => Settings.Id;

        public ImpulseSensorSettings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public VolumeFlow MaxValue => Settings.MaxValue;

        public string Accaury => Settings.Accaury;

        public void Dispose()
        {
            ownerDeviceDataCompliteDisposable?.Dispose();
        }

        public void SaveSettings()
        {
            _settingsService.SaveSensorSettings(settingsKey + Id, Settings);

            // Записываем текущее время, чтобы можно было узнать когда последний раз калибровался файл
            LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
            _settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
        }

        public void Reset()
        {
            oldCounter = counter;
            stopwatch.Restart();
        }

        public void Connect()
        {
            var isStart = false;
            ownerDeviceDataCompliteDisposable = ownerDevice.DataComplite.Subscribe(deviceChannel =>
            {
                if (deviceChannel.Id != Settings.ChannelId) return;
                if (deviceChannel.DataType == ChannelDataType.INT)
                {
                    if (!isStart)
                    {
                        isStart = true;
                        stopwatch.Restart();
                    }
                    counter = Convert.ToUInt32(deviceChannel.Value);

                    Counter = counter - oldCounter;
                    if (stopwatch.Elapsed.TotalSeconds > 1.0)
                        Flow = new VolumeFlow(filter.Filtering(Counter / stopwatch.Elapsed.TotalSeconds * 60.0 * Settings.ImpulsePrice.CubicCentimeters), VolumeFlowUnit.CubicCentimeterPerMinute).ToUnit(Settings.FlowUnit);
                    else Flow = new VolumeFlow(0, VolumeFlowUnit.CubicCentimeterPerMinute).ToUnit(Settings.FlowUnit);
                    //Debug.WriteLine(stopwatch.Elapsed.TotalSeconds);
                    Notify(Flow);
                }
            });
            stopwatch.Restart();
        }

        public void Disconnect()
        {
            ownerDeviceDataCompliteDisposable?.Dispose();
        }
    }
}
