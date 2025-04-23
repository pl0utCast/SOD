using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.App.Testing.Standarts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Linq;
using SOD.App.Commands;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class ProgrammMethodicsInfoViewModel
    {
        public ProgrammMethodicsInfoViewModel(ProgrammMethodicsConfig programmMethodicsConfig, IEnumerable<LuaStandart> standarts, Action editAction)
        {
            Name = programmMethodicsConfig.Name;
            CreatedDate = programmMethodicsConfig.CreatedDate.ToString("dd.MM.yyyy hh:mm");
            foreach (var children in programmMethodicsConfig.Childrens)
            {
                if(children is CommandConfig commandConfig)
                { 
                    FullInfo.Add(commandConfig.ToString());
                }
                if (children is TestProgrammConfig testProgrammConfig)
                {
                    FullInfo.Add(new TestInfoViewModel(testProgrammConfig, standarts));
                }
            }
            Edit = ReactiveCommand.Create(() => editAction());

            Config = programmMethodicsConfig;
        }
        public ReactiveCommand<Unit, Unit> Edit { get; set; }
        public List<object> FullInfo { get; set; } = new List<object>();
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public ProgrammMethodicsConfig Config { get; set; }
    }
}
