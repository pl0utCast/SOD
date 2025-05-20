using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.App.Commands
{
    public interface ICommand : ITestItem
    {
        public CommandConfig CommandConfig { get; set; }
        Task ExecuteAsync(CancellationToken cancellationToken, bool isAuto, params object[] parameters);
        CommandType Type { get; }
    }
}
