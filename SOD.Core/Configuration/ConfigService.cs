using Newtonsoft.Json;
using NLog;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SOD.Core.Configuration
{
    public class ConfigService : IConfigService
    {
        private readonly string configDirectory = "Configs";
        private readonly string deviceConfigFileName = "device.json";
        private readonly string sensorConfigFileName = "sensor.json";
        private readonly ILogger logger = LogManager.GetLogger(CoreConst.LoggerName);
        public ConfigService()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), configDirectory)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), configDirectory));
            }
        }

        public IEnumerable<DeviceConfig> GetDeviceConfigs()
        {
            var deviceConfigsString = ReadFile(Path.Combine(Directory.GetCurrentDirectory(), configDirectory, deviceConfigFileName));
            if (deviceConfigsString == string.Empty) return new List<DeviceConfig>();
            try
            {
                return JsonConvert.DeserializeObject<List<DeviceConfig>>(deviceConfigsString);
            }
            catch (Exception e)
            {
                logger.Error(e, "Ошибка получения конфигов из файла {0}", deviceConfigFileName);
                return new List<DeviceConfig>();
            }
            
        }

        public IEnumerable<SensorConfig> GetSensorConfigs()
        {
            var sensorConfigsString = ReadFile(Path.Combine(Directory.GetCurrentDirectory(), configDirectory, sensorConfigFileName));
            if (sensorConfigsString == string.Empty) return new List<SensorConfig>();
            try
            {
                return JsonConvert.DeserializeObject<List<SensorConfig>>(sensorConfigsString);
            }
            catch (Exception e)
            {
                logger.Error(e, "Ошибка получения конфигов из файла {0}", sensorConfigFileName);
                return new List<SensorConfig>();
            }
        }

        private string ReadFile(string path)
        {
            if (!File.Exists(path)) return string.Empty;

            using (var sr  = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
