using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.Frenq
{
    public class FrenqSensor : BaseSensor<VolumeFlow>, ILeakageSensor, IDisposable
    {
        private IChannelBasedDevice ownerDevice;
        private readonly ISettingsService _settingsService;

        private string settingsKey = "ImpulseSensor_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private IDisposable ownerDeviceDataCompliteDisposable;
        private Stopwatch stopwatch = new Stopwatch();
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        private double oldCounter = 0;
        private double counter = 0;
        private DigitalFilter filter;

        public FrenqSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(settingsKey + id, new FrenqSensorSettings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            Settings.Id = id;
            ownerDevice = channelBasedDevice;
            timer.Elapsed += Timer_Elapsed;
            filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
            Connect();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Notify(Flow);
            oldCounter = counter;
            stopwatch.Restart();
        }

        public VolumeFlow Flow { get; private set; } = new VolumeFlow();

        public Volume Volume => new Volume(Counter * Settings.ImpulsePrice.CubicCentimeters, VolumeUnit.CubicCentimeter);
        public double Counter { get; private set; }

        public int Id => Settings.Id;

        public FrenqSensorSettings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public VolumeFlow MaxValue => Settings.MaxValue;

        public string Accaury => Settings.Accaury;

        public void Dispose()
        {
            ownerDeviceDataCompliteDisposable?.Dispose();
            timer.Stop();
            timer.Elapsed -= Timer_Elapsed;
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

                    Flow = new VolumeFlow(filter.Filtering(Counter /*/ stopwatch.Elapsed.TotalSeconds*/ * 60.0 * Settings.ImpulsePrice.CubicCentimeters), VolumeFlowUnit.CubicCentimeterPerMinute).ToUnit(Settings.FlowUnit);
                }
                //Для цифрового расходомера тип double и счетчик не нужен
                else if (deviceChannel.DataType == ChannelDataType.DOUBLE)
                {
                    if (!isStart)
                    {
                        isStart = true;
                        stopwatch.Restart();
                    }
                    counter = Math.Round(Convert.ToDouble(deviceChannel.Value), 3);
                    Counter = counter;

                    Flow = new VolumeFlow(filter.Filtering(Counter * Settings.ImpulsePrice.CubicCentimeters), VolumeFlowUnit.LiterPerMinute).ToUnit(Settings.FlowUnit);
                }
            });
            stopwatch.Restart();
            timer.Start();
        }

        public void Disconnect()
        {
            ownerDeviceDataCompliteDisposable?.Dispose();
        }
    }
}
