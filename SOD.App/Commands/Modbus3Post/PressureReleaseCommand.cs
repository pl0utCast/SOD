using MemBus;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages.Commands;
using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class PressureReleaseCommand : BaseCommand
    {
        private object[] _param;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        public PressureReleaseCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _param = param;
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            Type = CommandType.PressureRelease;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //if (_param[0] is int time)
            //{
            //    await _modbusTcpDevice.WriteInt32(40, 7);
            //    await _modbusTcpDevice.WriteInt32(42, time);
            //    await Start();
            //    await _bus.PublishAsync(new RegistrationMessage(RegistartionStatus.Start, time));

            //    if (_param[1] is bool timeInStandart)
            //    {
            //        if (timeInStandart) time = Convert.ToInt32((double)parameters[1]);
            //    }
            //    // ждём окончание регистрации
            //    await Task.Delay(TimeSpan.FromSeconds(time), cancellationToken);
            //    await _bus.PublishAsync(new RegistrationMessage(RegistartionStatus.End, time));
            //    await _modbusTcpDevice.WriteInt32(50, 1);
            //    // ожидаем окончание выполнения команды
            //    await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
            //        async data =>
            //        {
            //            logger.Trace("Команда регистрация выполнена!");
            //            await ExecuteEnd();
            //        },
            //        cancellationToken);
            //}
        }
    }
}
