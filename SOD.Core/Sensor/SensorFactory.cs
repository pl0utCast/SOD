using NLog;
using SOD.Core.Configuration;
using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Sensor
{
    public class SensorFactory
    {
        private readonly ISettingsService _settingsService;
        private readonly IDeviceService _deviceService;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISensorService _sensorService;
        public SensorFactory(ISettingsService settingsService, IDeviceService deviceService, ISensorService sensorService)
        {
            _settingsService = settingsService;
            _deviceService = deviceService;
            _sensorService = sensorService;
        }

        public ISensor CreateSensor(SensorConfig sensorConfig)
        {
            if (sensorConfig == null) return null;
            var device = _deviceService.GetDevice(sensorConfig.DeviceId);
            if (device == null)
            {
                logger.Warn("Устройства c ID: {0} заданного в конфиге сенсора с ID:{1} не существует!", sensorConfig.DeviceId, sensorConfig.Id);
                return null;
            }
            switch (sensorConfig.Type)
            {
                case SensorType.PressureSensor:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new PressureSensor.PressureSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.FrenqSensor:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new Frenq.FrenqSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.TemperatureSensor:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new TemperatureSensor.TemperatureSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.PressureSensorCodeBased:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new PressureSensor.CodeBased.PressureSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.LeakageSensorCodeBased:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new LeakageSensor.CodeBased.LeakageSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.TemperatureSensorCodeBased:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new TemperatureSensor.CodeBased.TemperatureSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
                case SensorType.ImpulseSensor:
                    {
                        if (device is IChannelBasedDevice channelBasedDevice)
                        {
                            return new LeakageSensor.Impulse.ImpulseSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
                        }
                        return null;
                    }
				case SensorType.TensoSensorCodeBased:
					{
						if (device is IChannelBasedDevice channelBasedDevice)
						{
							return new TensoSensor.CodeBased.TensoSensor(sensorConfig.Id, channelBasedDevice, _settingsService);
						}
						return null;
					}
				default: return null;
            }
        }
    }
}
