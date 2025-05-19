using MemBus;
using SOD.App.Messages.Commands;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.ModbusCommands
{
    public class EmptyingCellCommand : BaseCommand, ICommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;

        public EmptyingCellCommand(ModbusTcpDevice modbusTcpDevice,
                                   IBus bus,
                                   CommandConfig commandConfig,
                                   ILocalizationService localizationService,
                                   object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.EmptyingCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.EmptyingCell"]));

            ushort reg = 4127;
            ushort mask = (1 << 3); // Выставляем единицу в бит по счету (1 << 7)

            await _modbusTcpDevice.SetMaskWord(reg, mask);

            // Ожидаем от контроллера в бите с маской обнуления
            await _modbusTcpDevice.CreateTriggerAsync(reg, data => (data[0] & mask) == 0,
                async data =>
                {
                    //await ExecuteEnd();
                },
                cancellationToken);

            _bus.Publish(new StopExecuteCommand(false));

            logger.Trace("Команда опорожнения камеры выполнена!");
        }

    }
}
