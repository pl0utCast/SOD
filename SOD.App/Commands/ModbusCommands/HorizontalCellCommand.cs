using MemBus;
using SOD.App.Messages.Commands;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.ModbusCommands
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

            // Ожидаем от контроллера в бите с маской обнуления
            await _modbusTcpDevice.CreateTriggerAsync(reg, data => (data[0] & mask) == 0,
                async data =>
                {
                    //await ExecuteEnd();
                },
                cancellationToken);

            _bus.Publish(new StopExecuteCommand(false));

            logger.Trace("Команда установки камеры в горизонтальное положение выполнена!");
        }
    }
}
