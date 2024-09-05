using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SOD.Core.Device
{
    public class DeviceService : IDeviceService
    {
        private List<IDevice> devices = new List<IDevice>();
        public DeviceService(IConfigService configService, ISettingsService settingsService)
        {
            var deviceFactory = new DeviceFactory(settingsService);
            foreach (var deviceConfig in configService.GetDeviceConfigs())
            {
                var device = deviceFactory.CreateDevice(deviceConfig);
                if (device != null) devices.Add(device);
            }
        }
        public IEnumerable<IDevice> GetAllDevice()
        {
            return devices;
        }

        public IDevice GetDevice(int id)
        {
            return devices.SingleOrDefault(d => d.Id == id);
        }
    }
}
