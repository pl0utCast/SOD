using SOD.Dialogs;
using SOD.UserService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SOD.ViewModels.Settings.Users.Dialog
{
    public class EditUserViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly ObservableAsPropertyHelper<string> fullName;
        public EditUserViewModel(User user, IDialogService dialogService, IUserService userService)
        {
            User = user;
            FirstName = user.FirstName;
            SecondName = user.SecondName;
            Password = DecryptPassword(user.Password);
            fullName = this.WhenAnyValue(x => x.FirstName, x => x.SecondName)
                .Select(fn => fn.Item1 + " " + fn.Item2)
                .ToProperty(this, x => x.FullName);

            Save = ReactiveCommand.Create(() =>
            {
                user.FirstName = FirstName;
                user.SecondName = SecondName;
                user.Password = EncryptPassword(Password);
                userService.AddOrUpdate(user);
                dialogService.CloseAsync(true);
            });

            Cancel = ReactiveCommand.Create(() =>
            {
                dialogService.CloseAsync(false);
            });
        }

        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            string encryptPassword = string.Empty;

            //Шифруем пароль методом Цезаря со смещением в 11 знаков
            for (int i = 0; i < password.Length; i++)
                encryptPassword += (char)(((int)password[i]) + 11);

            return encryptPassword;
        }

        private string DecryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            string decryptPassword = string.Empty;

            //Дешифруем пароль методом Цезаря со смещением в 11 знаков
            for (int i = 0; i < password.Length; i++)
                decryptPassword += (char)(((int)password[i]) - 11);

            return decryptPassword;
        }

        public string FullName => fullName.Value;
        [Reactive]
        public string FirstName { get; set; }
        [Reactive]
        public string SecondName { get; set; }
        [Reactive]
        public string Password { get; set; }
        public User User { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
