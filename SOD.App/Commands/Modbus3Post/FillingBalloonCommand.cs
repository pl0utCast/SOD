using MemBus;
using NLog;
using SOD.Core.Device.Modbus;
using SOD.Core.Valves;
using SOD.App.Messages;
using SOD.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public class FillingBalloonCommand : BaseCommand
    {
        private object[] _param;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private readonly IBus _bus;
        public FillingBalloonCommand(ModbusTcpDevice modbusTcpDevice, IBus bus, CommandConfig commandConfig, params object[] param) : base(modbusTcpDevice, commandConfig)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _bus = bus;
            _param = param;
            Type = CommandType.FillingBalloon;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken, object[] parameters)
        {
            if (_param[0] is bool isManual)
            {
                //var time = 0;
                //if (isManual)
                //{
                //    time = (int)_param[1];
                //}
                //else
                //{
                //    var valveFillingTime = _valve.GetValveProperty("filling_time")?.Value;
                //    if (valveFillingTime!=null)
                //    {
                //        time = (int)valveFillingTime;
                //    }
                //}
                //_bus.Publish(new ValveFilling(isManual, time));
                //await _modbusTcpDevice.WriteInt32(40, 2);
                //await _modbusTcpDevice.WriteInt32(42, time);
                //await Start();
                //// ждём подтверждение от пользователя об открытии арматуры
                //var wait = true;
                //var dis = _bus.Subscribe<OpenManualValve>(m => wait = false);
                //while (wait)
                //{
                //    await Task.Delay(1000, cancellationToken);
                //}
                //dis.Dispose();
                //// пользователь открыл арматуру
                //await _modbusTcpDevice.WriteInt32(50, 1);
                //// ждём от контроллера ответа что арматура заполнена, заполняем магистрали стенда
                //await _modbusTcpDevice.CreateTriggerAsync(52, data => data[0] == 1,
                //    data =>
                //    {
                //        _bus.Publish(new ValveFill());
                //    },
                //    cancellationToken);
                //// ожидаем оконачние выполнения команды
                //await _modbusTcpDevice.CreateTriggerAsync(48, data => data[0] == 1, 
                //    async data => 
                //    { 
                //        logger.Trace("Команда заполнение выполнена!");
                //        await ExecuteEnd();
                //    },
                //    cancellationToken);
                
            }
        }
    }
}
