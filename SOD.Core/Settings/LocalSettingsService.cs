using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SOD.Core.Cryptography;
using SOD.Core.Infrastructure;
using SOD.Core.JsonConverters;
using SOD.Core.Sensor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnitsNet.Serialization.JsonNet;

namespace SOD.Core.Settings
{
	public class LocalSettingsService : ISettingsService
	{
		private JsonSerializer serializer = new JsonSerializer();
		private Dictionary<string, object> deviceSettingsDic = new Dictionary<string, object>();
		private Dictionary<string, object> sensorSettingsDic = new Dictionary<string, object>();
		private Dictionary<string, object> applicationSettingsDic = new Dictionary<string, object>();
		private readonly ILogger logger = LogManager.GetLogger(CoreConst.LoggerName);

		const string SettingsDeviceFileName = "deviceSettings.json";
		const string SettingsSensorFileName = "sensorSettings.json";
		const string ApplicationSettingsFileName = "applicationSettings.json";
		const string SettingsDirectory = "settings";
		public LocalSettingsService()
		{
			if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory)))
			{
				Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory));
			}
			serializer.Converters.Add(new TypeObjectDictonaryJsonConverter());
			serializer.Converters.Add(new UnitsNetJsonConverter());
			serializer.Converters.Add(new PropertyJsonConverter());
			// сохраняем предыдущие версии файлов
			CreateBackup(SettingsDeviceFileName);
			CreateBackup(SettingsSensorFileName);
			CreateBackup(ApplicationSettingsFileName);

			void CreateBackup(string filename)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, filename);
				var backup = "backup";
				if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, backup)))
				{
					Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, backup));
				}
				if (File.Exists(filePath))
				{
					var newFileName = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + "_" + filename + "." + backup;
					File.Copy(filePath, Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, backup, newFileName));
				}
			}
			LoadSettings();
		}
		public TSettings GetSettings<TSettings>(string settingsKey, TSettings defaultValue)
		{
			if (string.IsNullOrWhiteSpace(settingsKey)) throw new ArgumentNullException(nameof(settingsKey));
			if (defaultValue == null) throw new ArgumentNullException(nameof(defaultValue));

			var attributes = typeof(TSettings).GetCustomAttributes().ToArray();
			if (attributes.Count() == 0) return defaultValue;

			foreach (var attribute in attributes)
			{
				switch (attribute)
				{
					case DeviceSettingsAttribute:
						{
							var settings = GetSettings<TSettings>(settingsKey, deviceSettingsDic);
							if (settings == null) break;
							return settings;
						}
					case SensorSettingsAttribute:
						{
							var settings = GetSettings<TSettings>(settingsKey, sensorSettingsDic);
							if (settings == null) break;
							return settings;
						}
					case ApplicationSettingsAttribute:
						{
							var settings = GetSettings<TSettings>(settingsKey, applicationSettingsDic);
							if (settings == null) break;
							return settings;
						}
				}
			}
			return defaultValue;
		}

		public void SaveSettings<TSettings>(string settingsKey, TSettings settings)
		{
			if (string.IsNullOrWhiteSpace(settingsKey)) throw new ArgumentNullException(nameof(settingsKey));
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			var attributes = settings.GetType().GetCustomAttributes().ToArray();
			if (attributes.Count() == 0) return;

			switch (attributes.Last())
			{
				case DeviceSettingsAttribute settingsDeviceAttribute:
					{
						AddSettings(settingsKey, settings, deviceSettingsDic);
						break;
					}
				case SensorSettingsAttribute settingsSensorAttribute:
					{
						AddSettings(settingsKey, settings, sensorSettingsDic);
						break;
					}
				case ApplicationSettingsAttribute applicationSettingsAttribute:
					{
						AddSettings(settingsKey, settings, applicationSettingsDic);
						break;
					}
				default: return;
			}
			SaveSettings();
		}

		public void SaveSensorSettings<TSettings>(string settingsKey, TSettings settings)
		{
			if (string.IsNullOrWhiteSpace(settingsKey)) throw new ArgumentNullException(nameof(settingsKey));
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			var attributes = settings.GetType().GetCustomAttributes().ToArray();
			if (attributes.Count() == 0) return;

			AddSettings(settingsKey, settings, sensorSettingsDic);

			SaveSensorSettings();
		}

		private void AddSettings<TSettings>(string key, TSettings settings, Dictionary<string, object> dic)
		{
			try
			{
				if (!dic.ContainsKey(key))
				{
					dic.Add(key, SerializeSettings(settings));
				}
				else
				{

					dic[key] = SerializeSettings(settings);
				}
			}
			catch (Exception e)
			{
				logger.Warn(e, "Невозможно добавить настройки");
			}
		}

		private TSettings GetSettings<TSettings>(string key, Dictionary<string, object> dic)
		{
			try
			{
				if (dic.TryGetValue(key, out var settingsSerialize))
				{
					return DesirializeSettings<TSettings>(settingsSerialize);
				}

			}
			catch (Exception e)
			{
				logger.Warn(e, "Невозможно получить настройки");
			}
			return default(TSettings);
		}

		private object SerializeSettings<TSettings>(TSettings settings)
		{

			return JObject.FromObject(settings, serializer);
		}

		private TSettings DesirializeSettings<TSettings>(object settings)
		{
			return ((JObject)settings).ToObject<TSettings>(serializer);
		}

		private string SerializeDictonaty(Dictionary<string, object> dictonary)
		{
			return JsonConvert.SerializeObject(dictonary, Formatting.Indented);
		}
		private Dictionary<string, object> DesirializeDctonary(string serializeSettingsDic)
		{
			if (string.IsNullOrEmpty(serializeSettingsDic)) return null;
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(serializeSettingsDic);
		}

		public void LoadSettings()
		{
			var deviceSettings = DesirializeDctonary(Read(GetDeviceSettingsPath()));
			if (deviceSettings != null) deviceSettingsDic = deviceSettings;

			var sensorSettings = DesirializeDctonary(Read(GetSensorSettingsPath()));
			if (sensorSettings != null) sensorSettingsDic = sensorSettings;

			var applicationSettings = DesirializeDctonary(Read(GetApplicationSettingsPath()));
			if (applicationSettings != null) applicationSettingsDic = applicationSettings;
			logger.Info("Загрузили настройки.");
		}

		public void SaveSettings()
		{
			var deviceSettings = SerializeDictonaty(deviceSettingsDic);
			Write(GetDeviceSettingsPath(), deviceSettings);

			var sensorSettings = SerializeDictonaty(sensorSettingsDic);
			Write(GetSensorSettingsPath(), sensorSettings);

			var applicationSettings = SerializeDictonaty(applicationSettingsDic);
			Write(GetApplicationSettingsPath(), applicationSettings);

			logger.Info("Сохранили настройки");
		}

		public void SaveSensorSettings()
		{
			var sensorSettings = SerializeDictonaty(sensorSettingsDic);
			Write(GetSensorSettingsPath(), sensorSettings);

			logger.Info("Сохранили настройки датчиков");

			// Бэкапим сохраненный файл
			CreateBackupSensorSettings("sensorSettings.json");
			// Сохраняем контрольную сумму в отдельный файл
			SaveCRCToFile();
		}

		public void SaveCRCToFile()
		{
			// Находим контрольную сумму файла
			int findCRCFromFile = new CRC(CRCCode.CRC32).FindCRC32(Path.Combine(Directory.GetCurrentDirectory(), "settings", "sensorSettings.json"));
			string checkSumPath = Path.Combine(Directory.GetCurrentDirectory(), "settings", "sensorSettingsCheckSum.json");

			try
			{
				if (!File.Exists(checkSumPath))
					using (File.Create(checkSumPath)) { }

				using (StreamWriter sw = new(checkSumPath))
				{
					sw.WriteLine(findCRCFromFile.ToString("X2"));
				}
			}
			catch (Exception)
			{
				logger.Warn(string.Format("Ошибка записи в файл - {0}", checkSumPath));
			}
		}

		void CreateBackupSensorSettings(string filename)
		{
			try
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings", filename);
				if (File.Exists(filePath))
				{
					var newFileName = Path.Combine(Directory.GetCurrentDirectory(), "settings", "backup" + "_" + filename);

					if (File.Exists(newFileName))
						File.Delete(newFileName);
					File.Copy(filePath, newFileName);
				}
			}
			catch (Exception)
			{
				logger.Warn(string.Format("Ошибка при копировании файла - {0}", filename));
			}
		}

		private string Read(string path)
		{
			if (!File.Exists(path))
			{
				logger.Warn(string.Format("Файл по пути  - {0} не найден.", path));
				return string.Empty;
			}
			using (StreamReader sr = new StreamReader(path))
			{
				try
				{
					return sr.ReadToEnd();
				}
				catch (Exception e)
				{
					logger.Warn(e, "Ошибка при четния из файла " + nameof(path));
				}
			}
			return string.Empty;
		}

		private void Write(string path, string fileData)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(path))
				{
					sw.Write(fileData);
				}
			}
			catch (Exception e)
			{
				logger.Warn(e, string.Format("Ошибка записи в файл - {0}", path));
			}

		}
		public static string GetDeviceSettingsPath() => Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, SettingsDeviceFileName);
		public static string GetSensorSettingsPath() => Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, SettingsSensorFileName);
		public static string GetApplicationSettingsPath() => Path.Combine(Directory.GetCurrentDirectory(), SettingsDirectory, ApplicationSettingsFileName);
	}
}
