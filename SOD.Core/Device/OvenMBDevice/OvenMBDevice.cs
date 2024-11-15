using NLog;
using NModbus;
using NModbus.Serial;
using NModbus.Utility;
using SOD.Core.Infrastructure;
using System.IO.Ports;
using System.Reactive.Subjects;

namespace SOD.Core.Device.OvenMBDevice
{
    public class OvenMBDevice : IDevice, IChannelBasedDevice
    {
        private IModbusMaster modbusMaster;
        private DeviceStatus deviceStatus = DeviceStatus.Disconnect;
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private Subject<DeviceStatus> deviceStatusSubject = new Subject<DeviceStatus>();
        private Subject<IDeviceChannel> dataComplitSubject = new Subject<IDeviceChannel>();
        private CancellationTokenSource cancellationConnect;
        private readonly ISettingsService _settingsService;
        private readonly string settingsKey = "OvenMBDevice_Id_";
        private SerialPort serialPort;
        public OvenMBDevice(int id, ISettingsService settingsService)
        {
            Id = id;
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(settingsKey + id, new Settings());
        }


        public void Connenct()
        {
            if (deviceStatus == DeviceStatus.Online) return;
            if (string.IsNullOrEmpty(Settings.SerialPort)) return;
            cancellationConnect = new CancellationTokenSource();
            serialPort?.Dispose();
            Task.Run(async () => await PoolingAsync());
        }

        public void Disconnect()
        {
            cancellationConnect?.Cancel();
            if (serialPort != null && serialPort.IsOpen) serialPort.Close();
        }

        public DeviceStatus GetStatus() => deviceStatus;

        public void SaveSettings()
        {
            _settingsService.SaveSettings(settingsKey + Id, Settings);
        }

        private async Task PoolingAsync()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = Settings.UsedBaudRate; //9600
            serialPort.Parity = Parity.Even;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.ReadTimeout = 100;
            while (!cancellationConnect.IsCancellationRequested)
            {
                try
                {
                    serialPort.PortName = Settings.SerialPort;
                    serialPort.Open();
                    if (!serialPort.IsOpen) SetStatus(DeviceStatus.Error);
                    else SetStatus(DeviceStatus.Online);
                    logger.Info($"Открыли COM порт {Settings.SerialPort}");
                    modbusMaster = new ModbusFactory().CreateRtuMaster(serialPort);
                    while (deviceStatus == DeviceStatus.Online)
                    {
                        foreach (var rChannel in Settings.RequestChannels)
                        {
                            try
                            {
                                if (rChannel.IsEnable)
                                {
                                    var registers = modbusMaster.ReadHoldingRegisters(1, rChannel.Address, 2);
                                    var impulse = ModbusUtility.GetSingle(registers[0], registers[1]);
                                    dataComplitSubject.OnNext(new DeviceChannel() { Id = rChannel.Address, DataType = ChannelDataType.FLOAT, Value = impulse });
                                }
                            }
                            catch (InvalidOperationException te)
                            {
                                break;
                            }
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (Exception e)
                {
                    SetStatus(DeviceStatus.Error);
                    if (serialPort.IsOpen) serialPort.Close();
                }


                await Task.Delay(3000);
            }
        }

        private void SetStatus(DeviceStatus status)
        {
            deviceStatus = status;
            deviceStatusSubject.OnNext(status);
        }

        public Settings Settings { get; set; }
        public string Name => Settings.Name;
        public string SensorHint => Settings.SensorHint;

        public int Id { get; private set; }

        public IObservable<DeviceStatus> Status => deviceStatusSubject;

        public IObservable<IDeviceChannel> DataComplite => dataComplitSubject;
    }
}
