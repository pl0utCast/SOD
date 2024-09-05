using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Core.Device.Modbus
{
    public class Request
    {
        public bool IsRepetable { get; set; }
        public Action RequsetTask { get; set; }
    }
}
