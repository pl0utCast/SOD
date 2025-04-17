using MemBus;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class PressureSetCommand : BaseCommand, ICommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        public PressureSetCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            Type = CommandType.PressureSet;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //await _modbusTcpDevice.WriteInt32(40, 8);
            //await Start();
            //// ожидаем оконачние выполнения команды
            //await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
            //    async data =>
            //    {
            //        logger.Trace("Команда сброс давления выполнена выполнена!");
            //        _bus.Publish(new PressureReleaseMessage());
            //        await ExecuteEnd();
            //    },
            //    cancellationToken);
        }
    }
}
