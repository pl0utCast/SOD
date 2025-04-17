using MemBus;
using NLog;
using SOD.Core.Device.Modbus;
using SOD.App.Mediums;
using SOD.App.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class HorizontalCellCommand : BaseCommand
    {
        private object[] _param;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        public HorizontalCellCommand(ModbusTcpDevice modbusTcpDevice,
                                 IBus bus,
                                 CommandConfig commandConfig,
                                 params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _param = param;
            Type = CommandType.HorizontalCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //if (_param[0] is MediumType mediumType)
            //{
            //    await _modbusTcpDevice.WriteInt32(40, 1);
            //    await _modbusTcpDevice.WriteHoldingRegistersAsync(42, new ushort[] {  (ushort)mediumType});
            //    await Start();
            //    await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1, 
            //        async data => 
            //        { 
            //            logger.Trace("Команда выбор среды выполнена!");
            //            await ExecuteEnd();
            //        },
            //        cancellationToken);
            //    await _bus.PublishAsync(new ChangeTestMedium(mediumType));
            //}
        }
    }
}
