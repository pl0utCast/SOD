using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SOD.Core.Reports
{
    public class RawReport
    {
        public int Id { get; set; }
        public Stream Report { get; set; }
    }
}
