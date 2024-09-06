using SOD.App.Commands;
using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Programms
{
    public class TestProgrammConfig : ITestConfig
    {
        public int? StandartId { get; set; }
        public TestType? TestType { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool CanAddChildren { get; private set; } = true;
        public List<KeyValuePair<Type, object>> Parameters { get; set; } = new List<KeyValuePair<Type, object>>();

        public IList<IBranch<ITestConfig>> Childrens { get; set; } = new List<IBranch<ITestConfig>>();
    }
}
