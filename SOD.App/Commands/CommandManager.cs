using MemBus;
using NLog;
using SOD.Core.Device;
using SOD.Core.Device.Modbus;
using SOD.Core.Infrastructure;
using SOD.Core.Valves;
using SOD.App.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SOD.Core.Balloons;

namespace SOD.App.Commands
{
    public class CommandManager
    {
        private readonly ISettingsService _settingsService;
        private readonly IDevice _device;
        private readonly IBus _bus;
        private CommandsFactory commandsFactory;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public CommandManager(IDevice device, IBus bus, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _device = device;
            _bus = bus;
            LoadSettings();
        }
        public async Task ExecuteCommands(List<CommandConfig> commandConfigs, Balloon testingValve, CancellationToken cancellationToken) 
        {
            if (IsStarted)
            {
                if (IsPaused) IsPaused = false;
                return;
            }
            await Task.Run(async () =>
            {
                IsStarted = true;

                foreach (var commandConfig in commandConfigs)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();
                    _bus.Publish(new ExecuteTestCommand(commandConfig, true));
                    var command = commandsFactory?.Create(commandConfig, testingValve);
                    try
                    {
                        await command.ExecuteAsync(cancellationToken);
                    }
                    catch (OperationCanceledException e)
                    {
                        IsStarted = false;
                        logger.Info($"Выполнение команды {command.Type} отменено");
                        _bus.Publish(new ExecuteTestCommand(commandConfig, false));
                        throw e;
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, $"Ошибка выполнения команды {command.Type}");
                    }

                    while (IsPaused) await Task.Delay(1000);
                    _bus.Publish(new ExecuteTestCommand(commandConfig, false));
                }
                IsStarted = false;
            }, cancellationToken); 
        }

        public void LoadSettings()
        {
            Settings = _settingsService.GetSettings("CommandManagerSettings", new CommandManagerSettings());
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings("CommandManagerSettings", Settings);
        }
        public Subject<CommandConfig> CurrentCommand { get; } = new Subject<CommandConfig>();
        public CommandManagerSettings Settings { get; set; }
        public  bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
    }
}
