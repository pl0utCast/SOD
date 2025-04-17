using MemBus;
using NLog;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class EmptyingCellCommand : BaseCommand,  ICommand
    {
        private object[] _param;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        
        public EmptyingCellCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _param = param;
            Type = CommandType.EmptyingCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //if (_param[0] is CavityType[] cavityTypes)
            //{
                // запуск команды
                await _modbusTcpDevice.WriteInt32(40, 3);
                await Start();
                await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
                    async data =>
                    {
                        logger.Trace("Команда выбора полости подачи давления, выполнена!");
                        await ExecuteEnd();
                    },
                    cancellationToken);
                // оповещение об выбранной полости
                //await _bus.PublishAsync(new SelectedPressurizeCavity(cavityTypes));
            //}
            //else return;
        }

    }
}
