using SOD.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Commands
{
    [ApplicationSettings]
    public class CommandManagerSettings
    {
        public int DeviceId { get; set; }
        public CommandCollectionType CommandCollectionType { get; set; }
    }
}
