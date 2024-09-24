using NLog;
using NModbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SOD.Core.Infrastructure;
using System.Collections.Concurrent;

namespace SOD.Core.Device.Modbus
{

	public class ModbusTcpDevice : IDevice, IChannelBasedDevice
	{
		private IModbusMaster modbusMaster;
		private DeviceStatus deviceStatus = DeviceStatus.Disconnect;
		private ILogger logger = LogManager.GetLogger(CoreConst.LoggerName);
		private Subject<DeviceStatus> deviceStatusSubject = new Subject<DeviceStatus>();
		private Subject<IDeviceChannel> dataComplitSubject = new Subject<IDeviceChannel>();
		private CancellationTokenSource cancellationConnect;
		private readonly ISettingsService _settingsService;
		private readonly string settingsKey = "ModbusTcpDevice_";

		private ConcurrentQueue<Request> queueRequest = new ConcurrentQueue<Request>();
		public ModbusTcpDevice(int id, ISettingsService settingsService)
		{
			_settingsService = settingsService;
			Settings = _settingsService.GetSettings(settingsKey + id, new ModbusTcpSettings());
			Settings.Id = id;
		}
		public void Connenct()
		{
			if (deviceStatus == DeviceStatus.Online) return;
			if (string.IsNullOrEmpty(Settings.HostOrIp) || Settings.Port == 0) return;
			cancellationConnect = new CancellationTokenSource();
			// формируем запрос на опрашивание постоянных регистров
			if (Settings.Registers.Count > 0)
				queueRequest.Enqueue(CreateRequestFromMainRegisters());
			//!!!Закомитил подключение к TCP модбасу во благо рабочего 39 адреса в мип W!!!
			Task.Run(async () => await PoolingAsync());
		}

		public void Disconnect()
		{
			if (deviceStatus != DeviceStatus.Online) return;
			deviceStatus = DeviceStatus.Disconnect;
			cancellationConnect?.Cancel();

		}

		public DeviceStatus GetStatus()
		{
			return deviceStatus;
		}

		private async Task PoolingAsync()
		{
			while (!cancellationConnect.IsCancellationRequested)
			{
				try
				{
					using (var tcpClient = new TcpClient(Settings.HostOrIp, Settings.Port))
					{
						cancellationConnect.Cancel();
						deviceStatus = DeviceStatus.Online;
						deviceStatusSubject.OnNext(deviceStatus);
						var factory = new ModbusFactory();
						modbusMaster = factory.CreateMaster(tcpClient);
						logger.Info("Подключились к устройству ID: {0}", Id);
						//Settings.Registers.OrderBy(r => r.RegisterId);
						while (deviceStatus == DeviceStatus.Online)
						{
							if (queueRequest.Count > 0)
							{
								if (queueRequest.TryDequeue(out var request))
								{
									request?.RequsetTask.Invoke();
									if (request.IsRepetable) queueRequest.Enqueue(request);
								}
								await Task.Delay(50);
							}
							else
							{
								await Task.Delay(50);
							}

						}
					}
				}
				catch (SocketException e)
				{
					// подключение не установлено игнорурем и просто ждём следуюшее
					if (e.SocketErrorCode != SocketError.ConnectionRefused) throw e;

				}
				catch (IOException)
				{
					deviceStatus = DeviceStatus.Error;
					deviceStatusSubject.OnNext(deviceStatus);
					logger.Info("Отключились от устройства с ID: {0}", Id);
					break;
				}
				catch (Exception e)
				{
					throw e;
				}


				await Task.Delay(5000);
			}
			// перезапускаем подключение у устройству
			if (deviceStatus == DeviceStatus.Error) Connenct();
		}
		private Request CreateRequestFromMainRegisters()
		{
			var request = new Request();
			request.IsRepetable = true;
			var regs = Settings.Registers.ToArray();
			request.RequsetTask = () =>
			{
				ReadRegisters(regs);
			};
			return request;
		}

		public void ReadHoldingRegisters(ModbusRegister[] regs)
		{
			var request = new Request();
			request.IsRepetable = false;
			request.RequsetTask = () =>
			{
				ReadRegisters(regs);
			};

			queueRequest.Enqueue(request);
		}

		public void WriteHoldingRegister(ModbusRegister modbusRegister)
		{
			try
			{
				if (modbusRegister.Value == null) return;
				var request = new Request();
				request.IsRepetable = false;
				request.RequsetTask = () =>
				{
					var data = new ushort[8];
					if (modbusRegister.DataType == ChannelDataType.BOOL)
					{
						data = new ushort[] { Convert.ToUInt16((bool)modbusRegister.Value) };
					}
					else if (modbusRegister.DataType == ChannelDataType.DOUBLE)
					{
						var bytes = BitConverter.GetBytes((double)modbusRegister.Value);
						data = new ushort[]
						{
						BitConverter.ToUInt16(bytes, 0),
						BitConverter.ToUInt16(bytes, 2),
						BitConverter.ToUInt16(bytes, 4),
						BitConverter.ToUInt16(bytes, 6),
						};
					}
					else if (modbusRegister.DataType == ChannelDataType.FLOAT)
					{
						var bytes = BitConverter.GetBytes((float)modbusRegister.Value);
						data = new ushort[]
						{
						BitConverter.ToUInt16(bytes, 0),
						BitConverter.ToUInt16(bytes, 2)
						};
					}
					else if (modbusRegister.DataType == ChannelDataType.INT)
					{
						var bytes = BitConverter.GetBytes((int)modbusRegister.Value);
						data = new ushort[]
						{
						BitConverter.ToUInt16(bytes, 0),
						BitConverter.ToUInt16(bytes, 2)
						};
					}
					else if (modbusRegister.DataType == ChannelDataType.UINT)
					{
						var bytes = BitConverter.GetBytes((uint)modbusRegister.Value);
						data = new ushort[]
						{
						BitConverter.ToUInt16(bytes, 0),
						BitConverter.ToUInt16(bytes, 2)
						};
					}
					else if (modbusRegister.DataType == ChannelDataType.INT16)
					{
						var bytes = BitConverter.GetBytes((Int16)modbusRegister.Value);
						data = new ushort[] { BitConverter.ToUInt16(bytes) };
					}
					if (modbusRegister.DataType == ChannelDataType.UINT16)
					{
						var bytes = BitConverter.GetBytes((UInt16)modbusRegister.Value);
						data = new ushort[] { BitConverter.ToUInt16(bytes) };
					}
					modbusMaster.WriteMultipleRegisters((byte)Settings.SlaveAddress, (ushort)modbusRegister.Id, data);
				};

				queueRequest.Enqueue(request);
			}
			catch (Exception e)
			{

				logger.Warn(e, "Ошибка записи в регистры");
			}

		}

		public async Task<ushort[]> ReadHoldingRegistersAsync(ushort regId, ushort regCount)
		{
			if (deviceStatus != DeviceStatus.Online) return null;
			try
			{
				return await modbusMaster.ReadHoldingRegistersAsync((byte)Settings.SlaveAddress, regId, regCount);
			}
			catch (Exception e)
			{
				logger.Error(e, "Ошибка чтения");
			}
			return null;

		}

		public async Task WriteHoldingRegistersAsync(ushort regId, ushort[] data)
		{
			if (deviceStatus != DeviceStatus.Online) return;
			try
			{
				await modbusMaster.WriteMultipleRegistersAsync((byte)Settings.SlaveAddress, regId, data);
			}
			catch (Exception e)
			{
				logger.Error(e, "Ошибка записи");
			}
		}


		private void ReadRegisters(ModbusRegister[] regs)
		{
			if (regs.Count() > 0)
			{
				// сортируем
				var registers = regs.OrderBy(r => r.Id).ToArray();
				// формируем запросы
				var requsetRegisters = new List<ModbusRegister>();
				var requests = new List<List<ModbusRegister>>();
				requsetRegisters.Add(registers[0]);
				requests.Add(requsetRegisters);
				for (int i = 1; i < registers.Count(); i++)
				{
					// если количество регистров в запросе превышает 125, делаем новый запрос для текущего регистра
					if ((registers[i].Id - requsetRegisters[0].Id + registers[i].GetModbusRegisterBytesCount() / 2) > 125)
					{
						requsetRegisters = new List<ModbusRegister>();
						requsetRegisters.Add(registers[i]);
						requests.Add(requsetRegisters);
					}
					// если не превышает, добавляем регистр в текущий запрос
					else
					{
						requsetRegisters.Add(registers[i]);
					}
				}
				// делаем запросы
				for (int i = 0; i < requests.Count; i++)
				{
					// ищем сколько регистров необходимо запросить
					var regCount = requests[i][0].GetModbusRegisterBytesCount() / 2;
					for (int j = 1; j < requests[i].Count; j++)
					{
						if (regCount < (requests[i][j].Id - requests[i][0].Id + requests[i][j].GetModbusRegisterBytesCount() / 2))
						{
							regCount = requests[i][j].Id - requests[i][0].Id + requests[i][j].GetModbusRegisterBytesCount() / 2;
						}
					}

					var data = modbusMaster.ReadHoldingRegisters((byte)Settings.SlaveAddress, (ushort)requests[i][0].Id, (ushort)regCount);
					// оповещаем о прочитанных регистрах
					for (int j = 0; j < requests[i].Count; j++)
					{
						var reg = requests[i][j];
						var regPosition = reg.Id - requests[i][0].Id;
						var val = new object();
						if (reg.GetModbusRegisterBytesCount() == 2)
						{
							val = reg.ParseModbusValue(new ushort[] { data[regPosition] });
						}
						else if (reg.GetModbusRegisterBytesCount() == 4)
						{
							val = reg.ParseModbusValue(new ushort[] { data[regPosition], data[regPosition + 1] });
						}
						else if (reg.GetModbusRegisterBytesCount() == 8)
						{
							val = reg.ParseModbusValue(new ushort[] { data[regPosition], data[regPosition + 1], data[regPosition + 2], data[regPosition + 3] });
						}
						else if (reg.DataType == ChannelDataType.STRING)
						{
							val = reg.ParseModbusValue(data);
						}
						reg.Value = val;
						dataComplitSubject.OnNext(reg);
						/*if (reg.Value == null)
                        {
                            reg.Value = val;
                            dataComplitSubject.OnNext(reg);
                        }
                        else if (!reg.Value.Equals(val))
                        {
                            reg.Value = val;
                            dataComplitSubject.OnNext(reg);
                        }*/
					}

				}
			}
		}
		public void SaveSettings()
		{
			_settingsService.SaveSettings(settingsKey + Id, Settings);
		}

		public int Id => Settings.Id;
		public IObservable<DeviceStatus> Status => deviceStatusSubject;

		public ModbusTcpSettings Settings { get; set; }

		public IObservable<IDeviceChannel> DataComplite => dataComplitSubject;

		public string Name => Settings.DeviceName;

		public string SensorHint => Settings.SensorHint;
	}
}
