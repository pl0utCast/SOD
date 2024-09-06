using SOD.App.Benches;
using SOD.App.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages.Test
{
    public class ManualValidateTestResponse
    {
        public ManualValidateTestResponse()
        {

        }
        public List<Tuple<int,PostStatus>> Status { get; set; } = new List<Tuple<int,PostStatus>>();
    }
}
