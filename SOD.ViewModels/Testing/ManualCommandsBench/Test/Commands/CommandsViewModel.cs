using MemBus;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.App.Commands;
using SOD.App.Messages.Commands;
using SOD.Dialogs;
using System.Reactive.Disposables;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands
{
    public class CommandsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IBus _bus;
        private readonly IDialogService _dialogService;
        public CommandsViewModel(IBus bus, IDialogService dialogService)
        {
            _bus = bus;
            _dialogService = dialogService;
            this.WhenActivated(dis =>
            {
                bus.Subscribe<ExecuteTestCommand>(m =>
                {
                    if (m.IsExecute)
                    {
                        Command = CreateCommandViewModel(m.CommandConfig);
                        Command.Activator.Activate();
                        Command.Activator.DisposeWith(dis);
                    }
                        
                    IsExecute = m.IsExecute;
                })
                .DisposeWith(dis);
            });

        }

        private IActivatableViewModel CreateCommandViewModel(CommandConfig commandConfig)
        {
            switch (commandConfig.Type)
            {
                case CommandType.FillingBalloon:
                    return new FillingBalloonCommandViewModel(commandConfig);
                case CommandType.EmptyingBalloon:
                    return new EmptyingBalloonCommandViewModel(_bus, _dialogService);
                case CommandType.FillingCell:
                    return new FillingCellCommandViewModel(commandConfig, _bus);
                case CommandType.EmptyingCell:
                    return new EmptyingCellCommandViewModel(commandConfig, _bus);
                case CommandType.PressureSet:
                    return new PressureSetCommandViewModel(commandConfig);
                case CommandType.PressureRelease:
                    return new PressureReleaseCommandViewModel();
                case CommandType.VerticalCell:
                    return new VerticalCellCommandViewModel(commandConfig, _bus);
                case CommandType.HorizontalCell:
                    return new HorizontalCellCommandViewModel(commandConfig);
                default:
                    break;
            }
            return null;
        }
        [Reactive]
        public bool IsExecute { get; set; }
        [Reactive]
        public IActivatableViewModel Command { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
