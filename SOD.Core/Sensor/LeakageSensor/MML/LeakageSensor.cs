using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace SOD.Core.Sensor.LeakageSensor.MML
{
    public class LeakageSensor : BaseSensor<VolumeFlow>, ILeakageSensor
    {
        private readonly ISensorService _sensorService;
        private IDiscreteInOutDevice _discreteInOutDevice;
        private readonly ISettingsService _settingsService;
        private const string SETTINGS_KEY = "MmlLeakageSensor_Id_";
        private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
        private List<ILeakageSensor> leakageSensors = new List<ILeakageSensor>();
        private IDisposable sensorValueUpdaterDis;
        private IDiscreteInOutDevice.DiscreteChannel currentChannel;
        private object locker = new object();

        public LeakageSensor(int id, IDiscreteInOutDevice discreteInOutDevice, ISettingsService settingsService, ISensorService senorService)
        {
            Id = id;
            _sensorService = senorService;
            _discreteInOutDevice = discreteInOutDevice;
            _settingsService = settingsService;

            Settings = _settingsService.GetSettings(SETTINGS_KEY + Id, new Settings());
            LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
            UpdateSensors();
            Connect();
        }

        public void Reset()
        {
            foreach (var sensor in leakageSensors)
            {
                sensor.Reset();
            }
        }

        public void SaveSettings()
        {
            UpdateSensors();
            _settingsService.SaveSensorSettings(SETTINGS_KEY + Id, Settings);

            // Записываем текущее время, чтобы можно было узнать когда последний раз калибровался файл
            LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
            _settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
        }

        private void UpdateSensors()
        {
            lock (locker)
            {
                sensorValueUpdaterDis?.Dispose();
                leakageSensors.Clear();
                foreach (var sensorSettings in Settings.SensorsSettings)
                {
                    var sensor = _sensorService.GetSensor(sensorSettings.Id);
                    if (sensor != null && sensor is ILeakageSensor leakageSensor)
                    {
                        leakageSensors.Add(leakageSensor);
                    }
                }
                if (leakageSensors.Count == 0) return;
                var maxValueSensorSettings = Settings.SensorsSettings.OrderByDescending(s => s.MaxValue).FirstOrDefault();
                var maxValueSensor = leakageSensors.SingleOrDefault(s => s.Id == maxValueSensorSettings.Id);

                if (maxValueSensor != null)
                {
                    MaxValue = maxValueSensor.MaxValue;
                    ActiveSensorId = maxValueSensor.Id;
                    var channels = _discreteInOutDevice.GetOutChannels();
                    foreach (var sensor in leakageSensors)
                    {
                        var sensorSettinngs = Settings.SensorsSettings.SingleOrDefault(s => s.Id == sensor.Id);
                        var channel = channels.SingleOrDefault(channel => channel.Number == sensorSettinngs.DiscreteChannelId);
                        SetChannel(channel, false);
                    }
                    if (Settings.SensorsSettings.Count > 0)
                    {
                        currentChannel = channels.SingleOrDefault(channel => channel.Number == Settings.SensorsSettings
                                                                                             .SingleOrDefault(s => s.Id == maxValueSensor.Id)
                                                                                             .DiscreteChannelId);
                        //SetChannel(currentChannel, true);
                        sensorValueUpdaterDis?.Dispose();
                        sensorValueUpdaterDis = maxValueSensor.Subscribe(flow =>
                        {
                            Flow = flow.ToUnit(Settings.Unit);
                            Notify(flow);
                        });
                    }

                    //Всегда включаем 4 выход по умолчанию
                    SetChannel(channels.FirstOrDefault(channel => channel.Number == 4), true);
                }
            }
        }

        private async Task ValueUpdaterAsync()
        {
            var channels = _discreteInOutDevice.GetOutChannels();
            while (true)
            {
                foreach (var sensorSettings in Settings.SensorsSettings.OrderByDescending(s => s.MaxValue.LitersPerMinute).Where(s => s.Id != ActiveSensorId))
                {
                    if (Flow >= sensorSettings.MinValue && Flow < sensorSettings.MaxValue)
                    {
                        var sensor = leakageSensors.SingleOrDefault(s => s.Id == sensorSettings.Id);
                        var channel = channels.SingleOrDefault(channel => channel.Number == sensorSettings.DiscreteChannelId);

                        if (Id == 12) //"PKTBA-MIP-A"
                        {
                            ActiveSensorId = sensor.Id;

                            //Логика для 5 датчиков
                            if (Settings.LogicSwitchMIP == 0)
                            {
                                switch (ActiveSensorId)
                                {
                                    case 5: //1000 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 6: //200 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 7: //10 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 8: //0.5 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 9: //0.05 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, false);
                                        break;
                                }
                            }
                            //Логика для 4 датчиков, которые отрываются по условию(несколько)
                            else if (Settings.LogicSwitchMIP == 1)
                            {
                                switch (ActiveSensorId)
                                {
                                    case 5: //1000 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 6: //25 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 7: //0.5 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 8: //0.05 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, false);
                                        break;
                                }
                            }
                            //Логика для 4 датчиков, которые открываются по одному
                            else if (Settings.LogicSwitchMIP == 2)
                            {
                                switch (ActiveSensorId)
                                {
                                    case 5: //1000 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, true);
                                        break;
                                    case 6: //200 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, false);
                                        break;
                                    case 7: //10 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, false);
                                        break;
                                    case 8: //0.5 l/min
                                        channel = channels.FirstOrDefault(channel => channel.Number == 0);
                                        SetChannel(channel, true);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 1);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 2);
                                        SetChannel(channel, false);

                                        channel = channels.FirstOrDefault(channel => channel.Number == 3);
                                        SetChannel(channel, false);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ActiveSensorId = sensor.Id;

                            SetChannel(currentChannel, false);
                            SetChannel(channel, true);
                        }

                        currentChannel = channel;
                        sensorValueUpdaterDis?.Dispose();
                        sensorValueUpdaterDis = sensor.Subscribe(flow =>
                        {
                            Flow = flow.ToUnit(Settings.Unit);
                            Notify(Flow);
                        });
                        await Task.Delay(sensorSettings.SwitchDelay);
                    }

                }
                await Task.Delay(100);
            }

        }

        private void SetChannel(IDiscreteInOutDevice.DiscreteChannel channel, bool state)
        {
            channel.IsEnable = state;
            _discreteInOutDevice.SetOutChannels(channel);
        }

        public void Connect()
        {
            Task.Run(async () => await ValueUpdaterAsync());
        }

        public void Disconnect()
        {
            var channels = _discreteInOutDevice.GetOutChannels();
            foreach (var sensorSettings in Settings.SensorsSettings)
            {
                var channel = channels.SingleOrDefault(channel => channel.Number == sensorSettings.DiscreteChannelId);
                SetChannel(channel, false);
            }
        }

        public int ActiveSensorId { get; private set; }

        public VolumeFlow Flow { get; private set; }

        public Volume Volume { get; private set; }

        public int Id { get; private set; }

        public string Name => Settings.Name;

        public string SensorHint => Settings.SensorHint;

        public Settings Settings { get; set; }
        public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }

        public VolumeFlow MaxValue { get; private set; } = new VolumeFlow(0, UnitsNet.Units.VolumeFlowUnit.CubicCentimeterPerMinute);

        public string Accaury => Settings.Accaury;
    }
}
