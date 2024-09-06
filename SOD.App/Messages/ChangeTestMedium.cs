using SOD.App.Mediums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Messages
{
    public class ChangeTestMedium
    {
        public ChangeTestMedium(MediumType mediumType)
        {
            Type = mediumType;
        }
        public MediumType Type { get; set; }
    }
}
