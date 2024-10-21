using SOD.Core.Device;
using SOD.Core.Infrastructure;
using SOD.Core.Sensor.PressureSensor;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TensoSensor
{
	public class TensoSensor : BaseSensor<Mass>, ITensoSensor
	{
		private readonly IChannelBasedDevice channelBasedDevice;
		private readonly ISettingsService _settingsService;
		private string settingsKey = "TensoSensor_";
		private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
		private IDisposable valueUpdateDisposable;
		public TensoSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
		{
			this.channelBasedDevice = channelBasedDevice;
			_settingsService = settingsService;
			Settings = _settingsService.GetSettings(settingsKey + id, new TensoSensorSettings());
			LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
			Settings.Id = id;
			Connect();

		}
		public Mass Mass { get; private set; }
		public int Id => Settings.Id;
		public TensoSensorSettings Settings { get; set; }
		public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }
		public string Name => Settings.Name;
		public string SensorHint => Settings.SensorHint;
		public Mass MaxValue => throw new NotImplementedException();
		public Mass MinValue => throw new NotImplementedException();
		public string Accaury => Settings.Accaury;

		public void Connect()
		{
			valueUpdateDisposable = channelBasedDevice.DataComplite.Subscribe(channel =>
			{
				if (channel.Id != Settings.ChannelId || channel.DataType != ChannelDataType.DOUBLE) return;
				Mass = new Mass((double)channel.Value, Settings.MassUnit);
				Notify(Mass);
			});
		}

		public void Disconnect()
		{
			valueUpdateDisposable?.Dispose();
		}

		public void SaveSettings()
		{
			_settingsService.SaveSensorSettings(settingsKey + Id, Settings);
			LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
			_settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
		}
	}

	public enum TensoSensorType
	{
		Tenso5,
		Tenso10,
		Tenso30,
	}
}
