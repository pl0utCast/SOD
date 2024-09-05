using SOD.Core.Valves.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SOD.Core.Valves
{
    public static class ValveExtension
    {
        public static ValveProperty GetValveProperty(this Valve valve, string prefix)
        {
            return valve.Properties.SingleOrDefault(vp => vp.Prefix == prefix);
        }

        public static void RemoveProperty(this Valve valve, int id)
        {
            var prop = valve.Properties.SingleOrDefault(p => p.Id == id);
            valve.Properties.Remove(prop);
        }
    }
}
