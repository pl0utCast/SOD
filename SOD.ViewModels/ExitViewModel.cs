using ReactiveUI;
using SOD.Core.Helpers;
using SOD.LocalizationService;
using SOD.UserService;
using System.Reactive;

namespace SOD.ViewModels
{
    public class ExitViewModel : ReactiveObject
    {
        public ExitViewModel(IUserService userService, ILocalizationService localizationService)
        {
            ViewTitle = localizationService["MainView.Exit"];
            ShoutDown = ReactiveCommand.Create(() => ApplicationExitHelper.ShoutDown());
            Reset = ReactiveCommand.Create(() => ApplicationExitHelper.Reset());
            Exit = ReactiveCommand.Create(() => ApplicationExitHelper.Exit());
        }

        public ReactiveCommand<Unit, Unit> Reset { get; set; }
        public ReactiveCommand<Unit, Unit> ShoutDown { get; set; }
        public ReactiveCommand<Unit, Unit> Exit { get; set; }
        public string ViewTitle { get; set; } = "Выход";
    }
}
