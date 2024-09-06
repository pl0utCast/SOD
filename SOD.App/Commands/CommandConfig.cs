using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.App.Commands
{
    public class CommandConfig : ITestConfig
    {
        public CommandType Type { get; set; }
        public List<KeyValuePair<Type, object>> Parameters { get; set; } = new List<KeyValuePair<Type, object>>();

        public int Id { get; set; }

        public bool CanAddChildren => false;


        public IList<IBranch<ITestConfig>> Childrens { get; } = new List<IBranch<ITestConfig>>();

        public override string ToString()
        {
            var result = Type.GetCommandName();
            result +=" "+this.GetParametersName();
            return result;
        }
    }
}
