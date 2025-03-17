using SOD.Core.Device;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TenzoSensor.CodeBased
{
	public class TenzoSensor : BaseSensor<Force>, ITenzoSensor, ICodeBasedSensor
	{
		private readonly IChannelBasedDevice channelBasedDevice;
		private readonly ISettingsService _settingsService;
		private const string SETTINGS_KEY = "TenzoSensorCodeBased_Id_";
		private string SETTINGS_LAST_UPDATE_KEY = "LastUpdate";
		private int code;
		private IDisposable disposable;
		private DigitalFilter filter;

		public TenzoSensor(int id, IChannelBasedDevice channelBasedDevice, ISettingsService settingsService)
		{
			Id = id;
			this.channelBasedDevice = channelBasedDevice;
			_settingsService = settingsService;
			Settings = _settingsService.GetSettings(SETTINGS_KEY + id, new Settings());
			LastUpadateSensorSettings = _settingsService.GetSettings(SETTINGS_LAST_UPDATE_KEY, new LastUpadateSensorSettings());
			Connect();
		}
        public Force Force { get; private set; }
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
                    code = Convert.ToInt32((float)dc.Value);

                    if (Settings.Coefficients.Count > 0)
                    {
                        int D = 0; // В мат. формуле идет отчет с 1, но мы то программисты - у нас массивы с нуля)

                        for (int i = 1; i < Settings.Coefficients.Count; i++) // Начинаем со второго по счету элемента
                        {
                            if (code > GetCode(i)) D++;
                        }

                        // Если выходит за пределы диапазона
                        if (D >= Settings.Coefficients.Count - 1) D = Settings.Coefficients.Count - 2;

                        double K = (GetWeight(D + 1) - GetWeight(D)) / (GetCode(D + 1) - GetCode(D));
                        double B = GetWeight(D) - (K * GetCode(D));
                        double Y = (K * code) + B;

                        Force = new Force(Y, ForceUnit.KilogramForce).ToUnit(Settings.Unit);
                        Notify(Force);
                    }
                }
            });
        }

		public void Disconnect()
		{
			disposable?.Dispose();
		}

        public double GetWeight(int el)
        {
            return Settings.Coefficients.ElementAt(el).InitialValue.KilogramsForce;
        }

        public int GetCode(int el)
        {
            return Settings.Coefficients.ElementAt(el).SavedCode;
        }

        public Settings Settings { get; set; }
		public LastUpadateSensorSettings LastUpadateSensorSettings { get; set; }
		public string Accaury => Settings.Accaury;
		public string SensorHint => Settings.SensorHint;
	}
}
