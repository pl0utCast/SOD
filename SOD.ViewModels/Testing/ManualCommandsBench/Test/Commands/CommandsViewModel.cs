using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Commands;
using SOD.App.Commands.ModbusCommands;
using SOD.App.Messages.Commands;
using SOD.Core.Device.Modbus;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.LocalizationService;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class CommandsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IBus _bus;
        private readonly IDialogService _dialogService;
        private readonly ILocalizationService _localizationService;
        private readonly ModbusTcpDevice _modbusTcpDevice;
        private CancellationTokenSource cancellationTokenSource;

        public CommandsViewModel(IBus bus, 
                                 IDialogService dialogService, 
                                 IDeviceService deviceService,
                                 ILocalizationService localizationService)
        {
            _bus = bus;
            _dialogService = dialogService;
            _localizationService = localizationService;
            _modbusTcpDevice = deviceService.GetAllDevice().FirstOrDefault(d => d is ModbusTcpDevice) as ModbusTcpDevice;
            cancellationTokenSource = new CancellationTokenSource();

            this.WhenActivated(dis =>
            {
                bus.Subscribe<ExecuteTestCommand>(async m =>
                {
                    if (m.IsExecute)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }

                        Command = CreateCommand(m.CommandConfig);
                        await Command.ExecuteAsync(cancellationTokenSource.Token, false);

                        //Command = CreateCommandViewModel(m.CommandConfig);
                        //Command.Activator.Activate();
                        //Command.Activator.DisposeWith(dis);
                    }
                        
                    IsExecute = m.IsExecute;
                })
                .DisposeWith(dis);
            });

        }

        private ICommand CreateCommand(CommandConfig commandConfig)
        {
            var parameters = commandConfig.Parameters.Select(kv => kv.Value).ToArray();
            switch (commandConfig.Type)
            {
                case CommandType.FillingBalloon:
                    return new FillingBalloonCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.EmptyingBalloon:
                    return new EmptyingBalloonCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.FillingCell:
                    return new FillingCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.EmptyingCell:
                    return new EmptyingCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.PressureSet:
                    return new PressureSetCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.PressureRelease:
                    return new PressureReleaseCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.VerticalCell:
                    return new VerticalCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.HorizontalCell:
                    return new HorizontalCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                default:
                    break;
            }
            return null;
        }

        //private IActivatableViewModel CreateCommandViewModel(CommandConfig commandConfig)
        //{
        //    switch (commandConfig.Type)
        //    {
        //        case CommandType.FillingBalloon:
        //            return new FillingBalloonCommandViewModel(commandConfig);
        //        case CommandType.EmptyingBalloon:
        //            return new EmptyingBalloonCommandViewModel(_bus, _dialogService);
        //        case CommandType.FillingCell:
        //            return new FillingCellCommandViewModel(commandConfig, _bus);
        //        case CommandType.EmptyingCell:
        //            return new EmptyingCellCommandViewModel(commandConfig, _bus);
        //        case CommandType.PressureSet:
        //            return new PressureSetCommandViewModel(commandConfig);
        //        case CommandType.PressureRelease:
        //            return new PressureReleaseCommandViewModel(commandConfig);
        //        case CommandType.VerticalCell:
        //            return new VerticalCellCommandViewModel(commandConfig, _bus);
        //        case CommandType.HorizontalCell:
        //            return new HorizontalCellCommandViewModel(commandConfig);
        //        default:
        //            break;
        //    }
        //    return null;
        //}

        [Reactive]
        public bool IsExecute { get; set; }
        [Reactive]
        public ICommand Command { get; set; }
        //public IActivatableViewModel Command { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
