using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Core.Helpers
{
    public static class ApplicationExitHelper
    {
        [DllImport("user32.dll")]
        private static extern int ExitWindowsEx(uint uFlags, uint dwReason);

        public static void Exit()
        {
            Environment.Exit(0);
        }

        public static void ShoutDown()
        {
            var shutDown = false;
            var flag = (uint)0;
            var command = "shutdown";
            var argument = "-f -t 0";
            flag |= (uint)0x01;
            shutDown = true;
            argument = "-s " + argument;

            if (!shutDown) return;

            var reason = (uint)0x80000000;
            var result = ExitWindowsEx(flag, reason);
            if (result == 0)
            {
                Process.Start(command, argument);
            }
        }

        public static void Reset()
        {
            var shutDown = false;
            var flag = (uint)0;
            var command = "shutdown";
            var argument = "-f -t 0";
            flag |= (uint)0x01;
            shutDown = true;
            argument = "-r " + argument;

            if (!shutDown) return;

            var reason = (uint)0x80000000;
            var result = ExitWindowsEx(flag, reason);
            if (result == 0)
            {
                Process.Start(command, argument);
            }
        }
    }
}
