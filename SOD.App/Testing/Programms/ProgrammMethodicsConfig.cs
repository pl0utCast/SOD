using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Programms
{
    public class ProgrammMethodicsConfig : ITestConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BalloonTypeId { get; set; }
        public bool CanAddChildren => true;

        public IList<IBranch<ITestConfig>> Childrens { get; set; } = new List<IBranch<ITestConfig>>();
    }
}
