using MemBus;
using NLog;
using SOD.Core.CustomUnits;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet;

namespace SOD.App.Commands.Modbus3Post
{
    public class VerticalCellCommand : BaseCommand, ICommand
    {
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;

        public VerticalCellCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            Type = CommandType.VerticalCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //if (_param[0] !=null)
            //{
            //    var pressure = 0.0;
            //    _bus.Publish(new SetPressure() { Pressure = pressure });
            //    await _modbusTcpDevice.WriteInt32(40, 5);
            //    await _modbusTcpDevice.WriteSingle(42, (float)pressure);
            //    await Start();
            //    await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1, 
            //        async data => 
            //        { 
            //            logger.Trace("Команда набора давления, выполнена!");
            //            await ExecuteEnd();
            //        },
            //        cancellationToken);
            //}
        }
    }
}
