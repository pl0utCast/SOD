using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Seals
{
    public static class SealHelper
    {
        public static string GetName(this SealType sealType)
        {
            switch (sealType)
            {
                case SealType.Metal:
                    return "металл-мeталл";
                case SealType.Polimer:
                    return "металл-полимер";
                default:
                    throw new NotSupportedException("Not support type " + sealType);
            }
        }
    }
}
