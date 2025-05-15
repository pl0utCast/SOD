using MemBus;
using SOD.App.Messages.Commands;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.Modbus3Post
{
    public class FillingCellCommand : BaseCommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;

        public FillingCellCommand(ModbusTcpDevice modbusTcpDevice,
                                  IBus bus,
                                  CommandConfig commandConfig,
                                  ILocalizationService localizationService,
                                  params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.FillingCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.FillingCell"]));

            ushort reg = 4127;
            ushort mask = (1 << 2); // Выставляем единицу в бит по счету (1 << 7)

            await _modbusTcpDevice.SetMaskWord(reg, mask, cancellationToken);

            _bus.Publish(new StopExecuteCommand(false));

            logger.Trace("Команда заполнения камеры выполнена!");
        }
    }
}
