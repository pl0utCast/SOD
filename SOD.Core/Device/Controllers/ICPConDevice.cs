using NLog;
using NModbus;
using SOD.Core;
using SOD.Core.Device;
using SOD.Core.Device.Modbus;
using SOD.Core.Infrastructure;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SOD.Core.Device.Controllers
{
    public class ICPConDevice : IDevice, IChannelBasedDevice, IDiscreteInOutDevice
    {
        private IModbusMaster modbusMaster;
        private DeviceStatus deviceStatus = DeviceStatus.Disconnect;
        private ILogger logger = LogManager.GetLogger(CoreConst.LoggerName);
        private Subject<DeviceStatus> deviceStatusSubject = new Subject<DeviceStatus>();
        private Subject<IDeviceChannel> dataComplitSubject = new Subject<IDeviceChannel>();
        private CancellationTokenSource cancellationConnect;
        private readonly ISettingsService _settingsService;
        private readonly string settingsKey = "ICPConDevice_";
        private ConcurrentQueue<Request> queueRequest = new ConcurrentQueue<Request>();
        private List<IDiscreteInOutDevice.DiscreteChannel> discreteChannels = new List<IDiscreteInOutDevice.DiscreteChannel>();

        public ICPConDevice(int id, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(settingsKey + id, new ModbusTcpSettings());
            Settings.Id = id;

            for (int i = 0; i < 4; i++)
            {
                discreteChannels.Add(new IDiscreteInOutDevice.DiscreteChannel() { IsEnable = false, Number = i });
            }
        }

        public void Connenct()
        {
            if (deviceStatus == DeviceStatus.Online) return;
            if (string.IsNullOrEmpty(Settings.HostOrIp) || Settings.Port == 0) return;
            cancellationConnect = new CancellationTokenSource();
            // формируем запрос на опрашивание постоянных регистров
            if (Settings.Registers.Count > 0)
                queueRequest.Enqueue(CreateRequestFromMainRegisters());
            Task.Run(async () => await PoolingAsync());
        }

        public void Disconnect()
        {
            if (deviceStatus != DeviceStatus.Online) return;

            // выключаем выхода
            foreach (var channel in GetOutChannels())
            {
                channel.IsEnable = false;
                SetOutChannels(channel);
            }

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

        public void ReadInputRegisters(ModbusRegister[] regs)
        {
            var request = new Request();
            request.IsRepetable = false;
            request.RequsetTask = () =>
            {
                ReadRegisters(regs);
            };

            queueRequest.Enqueue(request);
        }

        public void ReadHoldingRegistersRequest(ModbusRegister[] regs)
        {
            var request = new Request();
            request.IsRepetable = false;
            request.RequsetTask = () =>
            {
                ReadHoldingRegisters(regs);
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

        public async Task WriteSingleCoilAsync(ushort regId, bool data)
        {
            if (deviceStatus != DeviceStatus.Online) return;
            try
            {
                await modbusMaster.WriteSingleCoilAsync((byte)Settings.SlaveAddress, regId, data);
            }
            catch (Exception e)
            {
                logger.Error(e, "Ошибка записи");
            }
        }

        public void ReadCoils(ushort regId, ushort regCount)
        {
            if (deviceStatus != DeviceStatus.Online) return;
            try
            {
                var data = modbusMaster.ReadCoils((byte)Settings.SlaveAddress, regId, regCount);

                ModbusRegister reg = Settings.ReadWriteRegisters.FirstOrDefault(r => r.Id == regId);
                var val = reg.ParseModbusValue(data.Select(d => (ushort)(d ? 1 : 0)).ToArray());
                reg.Value = val;
                dataComplitSubject.OnNext(reg);
            }
            catch (Exception e)
            {
                logger.Error(e, "Ошибка чтения");
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

                    var data = modbusMaster.ReadInputRegisters((byte)Settings.SlaveAddress, (ushort)requests[i][0].Id, (ushort)regCount);
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

                        if (reg.DataType != ChannelDataType.BOOL)
                        {
                            reg.Value = val;
                            dataComplitSubject.OnNext(reg);
                        }
                    }
                }
            }
        }

        private void ReadHoldingRegisters(ModbusRegister[] regs)
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
                    }
                }
            }
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(settingsKey + Id, Settings);
            // Reconnect to device
            Disconnect();
            Connenct();
        }

        public void SetOutChannels(IDiscreteInOutDevice.DiscreteChannel discreteChannel)
        {
            if (deviceStatus != DeviceStatus.Online) return;
            try
            {
                modbusMaster.WriteSingleCoil((byte)Settings.SlaveAddress, (ushort)discreteChannel.Number, discreteChannel.IsEnable);
            }
            catch (Exception e)
            {
                logger.Error(e, "ICPConDevice -> SetOutChannels() -> Ошибка записи");
            }
        }

        public IEnumerable<IDiscreteInOutDevice.DiscreteChannel> GetOutChannels() => discreteChannels;
        public int Id => Settings.Id;
        public IObservable<DeviceStatus> Status => deviceStatusSubject;
        public ModbusTcpSettings Settings { get; set; }
        public IObservable<IDeviceChannel> DataComplite => dataComplitSubject;
        public string Name => Settings.DeviceName;
        public string SensorHint => Settings.SensorHint;
    }
}
