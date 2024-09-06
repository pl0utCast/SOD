using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.UserService
{
    public interface IUserService
    {
        public IEnumerable<User> GetUsers();
        public void AddOrUpdate(User user);
        public void Remove(User user);
        public bool Auth(User user, string password);
        public bool LogOut();
        public User GetCurrentUser();
        public IObservable<User> CurrentUser { get; }
        public string GetBackground();
        public bool GetAuthorization();
        public void SetBackground(string setBackground);
    }
}
