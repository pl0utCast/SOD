using SOD.App.Commands;
using SOD.App.Interfaces;
using SOD.App.Testing.Standarts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Programms
{
    public class TestProgramm : ITestItem
    {
        public int Id { get; set; }
        public bool CanAddChildren => true;
        public ITesting Test { get; set; }
        public IList<IBranch<ITestItem>> Childrens { get; } = new List<IBranch<ITestItem>>();
        public IStandart Standart { get; set; }
    }
}
