using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Helpers
{
    public static class LuaHelper
    {
        public static bool IsEquals(Enum first, Enum second)
        {
            return first.Equals(second);
        }
    }
}
