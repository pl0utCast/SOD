using SOD.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace SOD.UserService
{
    public class UserService : IUserService
    {
        private const string SETTINGS_KEY = "UserService";
        private readonly ISettingsService _settingsService;
        private Subject<User> userSubject = new Subject<User>();
        private User currentUser;

        public UserService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Settings = settingsService.GetSettings(SETTINGS_KEY, new Settings());
            if (Settings.Users.Count==0)
            {
                Settings.Users.Add(new User() { FirstName = "Администратор", Id = 1, Password = "957505", Role = UserRole.Administrator });
            }

        }
        public IObservable<User> CurrentUser => userSubject;

        public void AddOrUpdate(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var updateUser = Settings.Users.SingleOrDefault(u => u.Id == user.Id);
            if (updateUser == null)
            {
                user.Id = Settings.Users.Select(u => u.Id).Max() + 1;
                Settings.Users.Add(user);
                SaveSettings();
            }
            SaveSettings();
        }

        public bool Auth(User user, string password)
        {
            if (user.Password == password || (string.IsNullOrEmpty(user.Password) && string.IsNullOrEmpty(password)))
            {
                currentUser = user;
                userSubject.OnNext(user);
                isAutorization = true;
                return true;
            }
            return false;
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }

        public IEnumerable<User> GetUsers()
        {
            return Settings.Users;
        }

        public bool LogOut()
        {
            if (currentUser != null)
            {
                currentUser = null;
                userSubject.OnNext(currentUser);
                isAutorization = false;
                return true;
            }
            return false;
        }

        public void Remove(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            Settings.Users.Remove(user);
            SaveSettings();
        }

        public bool GetAuthorization()
        {
            return isAutorization;
        }

        private void SaveSettings()
        {
            _settingsService.SaveSettings(SETTINGS_KEY, Settings);
        }

        private Settings Settings { get; set; }
        private bool isAutorization { get; set; } = false;
    }
}
