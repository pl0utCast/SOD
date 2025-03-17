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
        public List<User> Users { get; set; } = new List<User>();
    }
}
