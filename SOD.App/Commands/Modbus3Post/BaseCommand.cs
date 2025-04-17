using NLog;
using SOD.Core.Device.Modbus;
using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands.Modbus3Post
{
    public abstract class BaseCommand : ICommand
    {
        private ModbusTcpDevice device;
        protected ILogger logger = LogManager.GetLogger(Const.LOGGER_NAME);

        public CommandType Type { get; protected set; }

        public int Id { get; set; }

        public IList<IBranch<ITestItem>> Childrens { get; } = new List<IBranch<ITestItem>>();

        public bool CanAddChildren => false;

        public BaseCommand(ModbusTcpDevice modbusTcpDevice, CommandConfig commandConfig)
        {
            device = modbusTcpDevice;
            CommandConfig = commandConfig;
        }
        /// <summary>
        /// Запуск команды на выполнение
        /// </summary>
        /// <returns></returns>
        protected async Task Start()
        {
            await device.WriteHoldingRegistersAsync(46, new ushort[] { 1 });
        }
        /// <summary>
        /// Остановка команды
        /// </summary>
        /// <returns></returns>
        protected async Task Stop()
        {
            await device.WriteHoldingRegistersAsync(46, new ushort[] { 0 });
        }

        /// <summary>
        /// Подтверждение о выполненой команде
        /// </summary>
        /// <returns></returns>
        protected async Task ExecuteEnd()
        {
            await device.WriteHoldingRegistersAsync(48, new ushort[] { 0 });
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken, params object[] parameters);
        public CommandConfig CommandConfig { get; set; }
    }
}
