using MemBus;
using SOD.Core.Device.Modbus;

namespace SOD.App.Commands.Modbus3Post
{
    public class PressureReleaseCommand : BaseCommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        public PressureReleaseCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            Type = CommandType.PressureRelease;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            // запускаем команду
            await _modbusTcpDevice.WriteInt32(40, 6);
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
