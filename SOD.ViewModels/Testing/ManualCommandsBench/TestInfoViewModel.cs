using SOD.App.Commands;
using SOD.App.Testing.Programms;
using SOD.App.Testing.Standarts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class TestInfoViewModel
    {
        public TestInfoViewModel(TestProgrammConfig config, IEnumerable<IStandart> standarts)
        {
            TestName = config.Name;
            StandartName = standarts.SingleOrDefault(s => s.Id == config.StandartId)?.Name;
            Commands = config.Childrens.Select(c => c.ToString()).ToList();
        }
        public string TestName { get; set; }
        public string StandartName { get; set; }
        public List<string> Commands { get; set; } = new List<string>();
    }
}
