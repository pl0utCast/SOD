using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SOD.Core.Sensor
{
    public class SensorService : ISensorService
    {
        private List<ISensor> sensors = new List<ISensor>();
        public SensorService(ISettingsService settingsService, IConfigService configService, IDeviceService deviceService)
        {
            var sensorFactory = new SensorFactory(settingsService, deviceService, this);
            foreach (var sensorConfig in configService.GetSensorConfigs())
            {
                var sensor = sensorFactory.CreateSensor(sensorConfig);
                if (sensor!=null)
                {
                    sensors.Add(sensor);
                }
            }
        }
        public IEnumerable<ISensor> GetAllSensors()
        {
            return sensors;
        }

        public ISensor GetSensor(int id)
        {
            return sensors.SingleOrDefault(s => s.Id == id);
        }
    }
}
