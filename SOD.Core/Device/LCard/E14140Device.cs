using NLog;
using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.Core.Device.LCard
{
    public class E14140Device : IDevice, IChannelBasedDevice, IDiscreteInOutDevice
    {
        private readonly ILogger logger = LogManager.GetLogger(CoreConst.LoggerName);
        private Subject<IDeviceChannel> dataCompliteSubject = new Subject<IDeviceChannel>();
        private Subject<DeviceStatus> deviceStatusSubject = new Subject<DeviceStatus>();
        private DeviceStatus deviceStatus;
        private readonly ISettingsService _settingsService;
        private const string SETTINGS_KEY = "E14_140_Settings_Id_";
        private Thread deviceThread;
        private IntPtr channelData;
        private Object locker = new object();
        private bool isInit = false;
        private CancellationTokenSource cancellationTokenSource;
        private int TTLOut = 0;
        private List<IDiscreteInOutDevice.DiscreteChannel> discreteChannels = new List<IDiscreteInOutDevice.DiscreteChannel>();
        
        //private const string dllPath = "D:/Source codes/SOD/x64/Debug/e140WrapperWindows.dll"; // Собираем новую версию библиотеки руками
        private const string dllPath = @"E140Wrapper.dll";

        [DllImport(dllPath)]
        private static extern int Initialization();
        [DllImport(dllPath)]
        private static extern bool ReadDataAsync(int chanelnumber, [In, Out] IntPtr data);
        [DllImport(dllPath)]
        private static extern bool set_ttl_out(int tll_out);
        [DllImport(dllPath)]
        private static extern bool Close();
        public E14140Device(int id, ISettingsService settingsService)
        {
            deviceStatus = DeviceStatus.Error;
            Id = id;
            _settingsService = settingsService;
            Settings = _settingsService.GetSettings(SETTINGS_KEY+Id, new E14140Settings());
            for (int i = 0; i < 16; i++)
            {
                discreteChannels.Add(new IDiscreteInOutDevice.DiscreteChannel() { IsEnable = false, Number = i });
            }
        }
        public void Connenct()
        {
            if (deviceStatus == DeviceStatus.Online) return;
            if (channelData!=null)
            {
                Marshal.FreeHGlobal(channelData);
                channelData = Marshal.AllocHGlobal(sizeof(short));
            }
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async ()=> await PullingLoop());
        }

        

        public DeviceStatus GetStatus()
        {
            return deviceStatus;
        }

        private void SetStatus(DeviceStatus status)
        {
            lock(locker)
            {
                deviceStatus = status;
                deviceStatusSubject.OnNext(status);
            }
        }

        private async Task PullingLoop()
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    // инциализировали устройство
                    var initCode = Initialization();
                    if (initCode >= 0) // Использовать if (initCode > 0) для новой прошивки e14140
                    {
                        isInit = true;
                        SetStatus(DeviceStatus.Online);
                        set_ttl_out(TTLOut);
                        while (deviceStatus == DeviceStatus.Online && !cancellationTokenSource.IsCancellationRequested)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                if (ReadDataAsync(i, channelData))
                                {
                                    //Marshal.WriteInt16(channelData, 0);
                                    var data = Marshal.ReadInt16(channelData);

                                    if (data < -10000)
                                    {
                                        isInit = false;
                                        SetStatus(DeviceStatus.Error);
                                        Close();
                                        logger.Warn($"Ошибка чтения канала с устройства Id - {Id}");
                                        break;
                                    }
                                    dataCompliteSubject.OnNext(new DeviceChannel() { Id = i, DataType = ChannelDataType.INT, Value = data });
                                }
                                else
                                {
                                    isInit = false;
                                    SetStatus(DeviceStatus.Error);
                                    if (isInit) Close();
                                    logger.Warn($"Ошибка чтения канала с устройства Id - {Id}");
                                    break;
                                }
                            }
                            await Task.Delay(5);
                        }
                    }
                    await Task.Delay(3000);
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                logger.Error(e, "Ошибка устройства");
                Disconnect();
                Connenct();
            }
        }

        public void Disconnect()
        {
            if (deviceStatus == DeviceStatus.Disconnect) return;
            // выключаем выхода
            foreach (var channel in GetOutChannels())
            {
                channel.IsEnable = false;
                SetOutChannels(channel);
            }

            SetStatus(DeviceStatus.Disconnect);
            if (isInit) Close();
            cancellationTokenSource.Cancel();
        }

        public IEnumerable<IDiscreteInOutDevice.DiscreteChannel> GetInChannel(IEnumerable<int> numbers)
        {
            throw new NotImplementedException();
        }

        public void SetOutChannels(IDiscreteInOutDevice.DiscreteChannel discreteChannel)
        {
            if (discreteChannel.IsEnable)
            {
                TTLOut &= ~(1 << discreteChannel.Number);
            }
            else
            {
                TTLOut |= (1 << discreteChannel.Number);
            }
            if (deviceStatus==DeviceStatus.Online)
            {
                set_ttl_out(TTLOut);
            }
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(SETTINGS_KEY + Id, Settings);
        }

        public IEnumerable<IDiscreteInOutDevice.DiscreteChannel> GetOutChannels() => discreteChannels;

        public IObservable<IDeviceChannel> DataComplite => dataCompliteSubject;
        public string Name => Settings.Name;
        public int Id { get; private set; }
        public IObservable<DeviceStatus> Status => deviceStatusSubject;
        public E14140Settings Settings { get; set; }

        public string SensorHint => Settings.SensorHint;
    }
}
