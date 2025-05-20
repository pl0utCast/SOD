using MemBus;
using SOD.App.Messages.Commands;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.ModbusCommands
{
    public class FillingBalloonCommand : BaseCommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;

        public FillingBalloonCommand(ModbusTcpDevice modbusTcpDevice,
                                     IBus bus,
                                     CommandConfig commandConfig,
                                     ILocalizationService localizationService,
                                     params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.FillingBalloon;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, bool isAuto, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.FillingBalloon"]));

            if (isAuto)
            {
                ushort regIdCommand = 4130;
                ushort idCommand = 4; // Id команды
                await _modbusTcpDevice.WriteHoldingRegistersAsync(regIdCommand, [idCommand]);

                // Старт команды и автоматический режим
                ushort regDebugManual = 4126; // D30.4, D30.6
                ushort maskDebugManual = ((1 << 4) | (1 << 6));
                await _modbusTcpDevice.SetOneMaskWord(regDebugManual, maskDebugManual);
            }
            else
            {
                // Не дебаг и не автоматический режим
                int regDebugManual = 4126; // D30.3, D30.4
                int maskDebugManual = ~((1 << 3) | (1 << 4)); // ~ инвертирует маску (пример для 1 и 2 регистра 11111001)
                await _modbusTcpDevice.SetZeroMaskWord(regDebugManual, maskDebugManual);
            }

            ushort reg = 4127;
            ushort mask = (1 << 0); // Выставляем единицу в бит по счету (1 << 7)
            await _modbusTcpDevice.SetOneMaskWord(reg, mask);

            // Ожидаем от контроллера в бите с маской обнуления
            await _modbusTcpDevice.CreateTriggerAsync(reg, data => (data[0] & mask) == 0,
                async data =>
                {
                    //await ExecuteEnd();
                },
                cancellationToken);

            _bus.Publish(new StopExecuteCommand(false));

            logger.Trace("Команда заполнения баллона выполнена!");
        }
    }
}
