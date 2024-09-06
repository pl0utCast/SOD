using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages
{
    public class EnablePostMessage
    {
        public EnablePostMessage(int id, bool isEnable)
        {
            Id = id;
            IsEnable = isEnable;
        }
        public int Id { get; set; }
        public bool IsEnable { get; set; }
    }
}
