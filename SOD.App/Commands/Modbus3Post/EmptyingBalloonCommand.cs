using MemBus;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages.Commands;
using SOD.App.Testing.Standarts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class EmptyingBalloonCommand : BaseCommand, ICommand
    {
        private object[] _param;
        private ModbusTcpDevice _modbusTcpDevice;
        private IBus _bus;
        public EmptyingBalloonCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _param = param;
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            Type = CommandType.EmptyingBalloon;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            if (_param[0] is int time)
            {
                // получение времени из стандарта
                if (_param[1] is bool timeInStandart)
                {
                    if (timeInStandart)
                    {
                        time = Convert.ToInt32((double)parameters[0]);
                    }
                }
                await _modbusTcpDevice.WriteInt32(40, 6);
                await _modbusTcpDevice.WriteInt32(42, time);
                await Start();
                _bus.Publish(new Hold(HoldStatus.Start, time));
                // ждём окончание выдержки
                await Task.Delay(TimeSpan.FromSeconds(time), cancellationToken);
                _bus.Publish(new Hold(HoldStatus.Stop, time));
                await _modbusTcpDevice.WriteInt32(50, 1);
                // ожидаем окончание выполнения команды
                await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
                    async data =>
                    {
                        logger.Trace("Команда выдержка выполнена!");
                        await ExecuteEnd();
                    },
                    cancellationToken);
            }
        }

    }
}
