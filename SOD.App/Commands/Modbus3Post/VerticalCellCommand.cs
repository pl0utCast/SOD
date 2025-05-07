using MemBus;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.Modbus3Post
{
    public class VerticalCellCommand : BaseCommand, ICommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;

        public VerticalCellCommand(ModbusTcpDevice modbusTcpDevice,
                                   IBus bus,
                                   CommandConfig commandConfig,
                                   ILocalizationService localizationService,
                                   params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.VerticalCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.VerticalCell"]));

            // запускаем команду
            await _modbusTcpDevice.WriteInt32(40, 7);
            await Start();
            //await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
            //    async data =>
            //    {
            //        logger.Trace("Команда выбор полости контроля утечки, выполнена!");
            //        await ExecuteEnd();
            //    },
            //    cancellationToken);
        }
    }
}
