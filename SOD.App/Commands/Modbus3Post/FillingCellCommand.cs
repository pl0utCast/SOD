using MemBus;
using SOD.Core.Device.Modbus;
using SOD.App.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class FillingCellCommand : BaseCommand
    {
        private object[] _param;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        public FillingCellCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _param = param;
            Type = CommandType.FillingCell;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            //if (_param[0] is CavityType[] cavityTypes)
            //{
                // запускаем команду
                await _modbusTcpDevice.WriteInt32(40, 4);
                await Start();
                await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1,
                    async data =>
                    {
                        logger.Trace("Команда выбор полости контроля утечки, выполнена!");
                        await ExecuteEnd();
                    },
                    cancellationToken);
                // выбрали полость
                //_bus.Publish(new SelectedLeakControlCavity(cavityTypes));
            //}
            //else return;
        }
    }
}
