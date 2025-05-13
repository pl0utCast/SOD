using MemBus;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.Modbus3Post
{
    public class HorizontalCellCommand : BaseCommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;

        public HorizontalCellCommand(ModbusTcpDevice modbusTcpDevice,
                                     IBus bus,
                                     CommandConfig commandConfig,
                                     ILocalizationService localizationService, 
                                     params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.HorizontalCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.HorizontalCell"]));

            ushort reg = 4126;
            ushort mask = (1 << 14); // Выставляем единицу в бит по счету (1 << 7)

            await _modbusTcpDevice.SetMaskWord(reg, mask);

            //await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
            //    async data =>
            //    {
            //        await ExecuteEnd();
            //    },
            //    cancellationToken);
            
            logger.Trace("Команда установки камеры в горизонтальное положение выполнена!");
        }
    }
}
