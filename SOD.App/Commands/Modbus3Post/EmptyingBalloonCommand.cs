using MemBus;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.Modbus3Post
{
    public class EmptyingBalloonCommand : BaseCommand, ICommand
    {
        private ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;
        private static ushort stopReg = 6118;

        public EmptyingBalloonCommand(ModbusTcpDevice modbusTcpDevice, 
                                      IBus bus, 
                                      CommandConfig commandConfig, 
                                      ILocalizationService localizationService,
                                      params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _localizationService = localizationService;
            Type = CommandType.EmptyingBalloon;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            _bus.Publish(new App.Benches.SODBench.Messages.InfoMessage(_localizationService["Testing.ManualCommandsBench.EmptyingBalloon"]));

            await _modbusTcpDevice.WriteInt32(40, 2);
            await Start();

            // ожидаем окончание выполнения команды
            await _modbusTcpDevice.CreateFloatTriggerAsync(stopReg, data => data == 1,
                async data =>
                {
                    logger.Trace("Команда выдержка выполнена!");
                    //await ExecuteEnd();
                },
                cancellationToken);
        }
    }
}
