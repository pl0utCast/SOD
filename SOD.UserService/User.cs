using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.UserService
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public UserRole Role { get; set; } = UserRole.Worker;
        public string Password { get; set; }
    }
}
