using SOD.ViewModels.Controls;
using SOD.Dialogs;
using SOD.Navigation;
using SOD.UserService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SOD.ViewModels.Settings.Users
{
    public class UsersSettingsViewModel : ReactiveObject
    {
        public UsersSettingsViewModel(INavigationService navigationService, IDialogService dialogService, IUserService userService)
        {
            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
            foreach (var user in userService.GetUsers())
            {
                Users.Add(new Dialog.EditUserViewModel(user, dialogService, userService));
            }

            Add = ReactiveCommand.CreateFromTask(async () =>
            {
                var user = new User();
                var vm = new Dialog.EditUserViewModel(user, dialogService, userService);
                var result = await dialogService.ShowDialogAsync("EditUser", vm);
                if (result!=null && (bool)result)
                {
                    Users.Add(vm);
                }
            });

            if (userService.GetCurrentUser().Role == UserRole.Worker) 
                Edit = ReactiveCommand.Create(() =>
                {
                    dialogService.ShowDialog("EditUser", SelectedUser);
                }, this.WhenAnyValue(x => x.SelectedUser).Select(u => u != null && u.User.Id == userService.GetCurrentUser().Id));

            if (userService.GetCurrentUser().Role != UserRole.Worker)
                Edit = ReactiveCommand.Create(() =>
                {
                    dialogService.ShowDialog("EditUser", SelectedUser);
                }, this.WhenAnyValue(x => x.SelectedUser).Select(u => u != null && u.User.Role != UserRole.Administrator));

            if (userService.GetCurrentUser().Role == UserRole.Worker)
                Delete = ReactiveCommand.CreateFromTask(async () =>
                {
                    var result = await dialogService.ShowDialogAsync("DeleteUser", new YesNoDialogViewModel(dialogService));
                    if (result!=null && (bool)result)
                    {
                        userService.Remove(SelectedUser.User);
                        Users.Remove(SelectedUser);
                    }
                }, this.WhenAnyValue(x => x.SelectedUser).Select(u => u != null && u.User.Id == userService.GetCurrentUser().Id));

            if (userService.GetCurrentUser().Role != UserRole.Worker)
                Delete = ReactiveCommand.CreateFromTask(async () =>
                {
                    var result = await dialogService.ShowDialogAsync("DeleteUser", new YesNoDialogViewModel(dialogService));
                    if (result != null && (bool)result)
                    {
                        userService.Remove(SelectedUser.User);
                        Users.Remove(SelectedUser);
                    }
                }, this.WhenAnyValue(x => x.SelectedUser).Select(u => u != null && u.User.Role != UserRole.Administrator));
        }
        public ObservableCollection<Dialog.EditUserViewModel> Users { get; set; } = new ObservableCollection<Dialog.EditUserViewModel>();
        [Reactive]
        public Dialog.EditUserViewModel SelectedUser { get; set; }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
    }
}
