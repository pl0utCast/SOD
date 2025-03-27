using MemBus;
using SOD.App.Commands;
using SOD.App.Messages.Commands;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

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
            //Command?.Activator.Deactivate();
            switch (commandConfig.Type)
            {
                case CommandType.TestMedium:
                    return new TestMediumCommandViewModel(commandConfig);
                case CommandType.Filling:
                    return new FillingCommandViewModel(_bus, _dialogService);
                case CommandType.PressurizedCavity:
                    return new PressurizeCavityCommandViewModel(commandConfig);
                case CommandType.LeakControlCavity:
                    return new LeakControlCavityViewModel(commandConfig);
                case CommandType.SetPressure:
                    return new SetPressureCommandViewModel(commandConfig, _bus);
                case CommandType.Hold:
                    return new HoldCommandViewModel(commandConfig, _bus);
                case CommandType.Registartion:
                    return new RegistartionCommandViewModel(commandConfig, _bus);
                case CommandType.PressureRelease:
                    return new PressureReleaseCommandViewModel();
                case CommandType.Purge:
                    return new PurgeCommandViewModel();
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
