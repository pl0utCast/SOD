using SOD.Core.Valves;
using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Testing.Programms
{
    public class ProgrammMethodics : ITestItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public ValveType ValveType { get; set; }
        public IList<IBranch<ITestItem>> Childrens { get; } = new List<IBranch<ITestItem>>();
        public bool CanAddChildren => true;
    }
}
