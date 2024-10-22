using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TensoSensor.CodeBased
{
	public class TensoSensor : BaseSensor<Mass>, ITensoSensor, ICodeBasedSensor
	{
		private readonly IChannelBasedDevice channelBasedDevice;
		private readonly ISettingsService _settingsService;
		private const string SETTINGS_KEY = "TensoSensorCodeBased_Id_";
		private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
		private int code;
		private IDisposable disposable;
		private DigitalFilter filter;

		public TensoSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
		{
			Id = id;
			this.channelBasedDevice = channelBasedDevice;
			_settingsService = settingsService;
			Settings = _settingsService.GetSettings(SETTINGS_KEY + id, new Settings());
			LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
			filter = new DigitalFilter() { Coeffecient = Settings.FilterCoef };
			Connect();
		}
		public Mass Mass { get; private set; }
		public int Id { get; private set; }
		public string Name => Settings.Name;
		public int Code => code;

		public void SaveSettings()
		{
			_settingsService.SaveSensorSettings(SETTINGS_KEY + Id, Settings);
			LastUpadateSensorSettings.LastUpdateDate = DateTime.Now;
			_settingsService.SaveSensorSettings(SETTINGS_LAST_UPDATE_KEY, LastUpadateSensorSettings);
		}

		public void Connect()
		{
			disposable = channelBasedDevice.DataComplite.Subscribe(dc =>
			{
				if (dc.Id == Settings.ChannelId && dc.DataType == ChannelDataType.FLOAT)
				{
					code = Convert.ToInt32((float)dc.Value * 1000); // Преобразую миливольты в вольты
					var rawValue = this.CodeToValue(Settings.MinValue.Grams, Settings.MaxValue.Grams, Settings.MinCode, Settings.MaxCode, code);
					rawValue = filter.Filtering(rawValue); // Фильтруем значение

					var k = Settings.MaxValue.Grams * 0.2 / 100;
					var n = Settings.MaxValue.Grams * -0.2 / 100;

					if (rawValue < k && rawValue > n)
					{
						Mass = new Mass(0, MassUnit.Gram).ToUnit(Settings.Unit);
					}
					else
					{
						Mass = new Mass(rawValue, MassUnit.Gram).ToUnit(Settings.Unit);
					}
					Notify(Mass);
				}
			});
		}

		public void Disconnect()
		{
			disposable?.Dispose();
		}

		public Settings Settings { get; set; }
		public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }
		public Mass MaxValue => Settings.MaxValue;
		public Mass MinValue => Settings.MinValue;
		public string Accaury => Settings.Accaury;
		public string SensorHint => Settings.SensorHint;
	}
}
