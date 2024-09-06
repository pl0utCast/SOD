using SOD.Core.Settings;
using System.Collections.Generic;

namespace SOD.UserService
{
    [ApplicationSettings]
    public class Settings
    {
        public Settings()
        {

        }
        public string SetBackground { get; set; } = "White";
        public List<User> Users { get; set; } = new List<User>();
    }
}
