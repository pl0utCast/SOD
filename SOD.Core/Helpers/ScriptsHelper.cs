using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SOD.Core.CoreConst;

namespace SOD.Core.Helpers
{
    public static class ScriptsHelper
    {
        public static string[] GetAllScripts()
        {
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), SCRIPT_DIRECTORY)))
            {
                return Directory
                    .GetFiles(Path.Combine(Directory.GetCurrentDirectory(), SCRIPT_DIRECTORY), "*.lua")
                    .Select(p => p.Replace(Path.Combine(Directory.GetCurrentDirectory(), SCRIPT_DIRECTORY), ""))
                    .Select(p => p.Remove(0, 1))
                    .ToArray();
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), SCRIPT_DIRECTORY));
                return new string[] { };
            }

        }
    }
}
