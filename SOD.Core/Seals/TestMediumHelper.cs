using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Seals
{
    public static class TestMediumHelper
    {
        public static string GetName(this TestMediumType sealType)
        {
            switch (sealType)
            {
                case TestMediumType.Water:
                    return "вода";
                case TestMediumType.Air:
                    return "воздух";
                case TestMediumType.Gas:
                    return "газ";
                default:
                    throw new NotSupportedException("Not support type " + sealType);
            }
        }
    }
}
