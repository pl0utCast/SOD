using MemBus;
using SOD.Core.Device.Modbus;

namespace SOD.App.Commands.Modbus3Post
{
    public class HorizontalCellCommand : BaseCommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        public HorizontalCellCommand(ModbusTcpDevice modbusTcpDevice,
                                 IBus bus,
                                 CommandConfig commandConfig,
                                 params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            Type = CommandType.HorizontalCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            // запуск команды
            await _modbusTcpDevice.WriteInt32(40, 8);
            await Start();
            //await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
            //    async data =>
            //    {
            //        logger.Trace("Команда выбора полости подачи давления, выполнена!");
            //        await ExecuteEnd();
            //    },
            //    cancellationToken);
        }
    }
}
