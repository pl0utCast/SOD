using SOD.Core.Valves.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Valves
{
    public class Valve : IEquatable<Valve>
    {
        public int Id { get; set; }
        public ValveType ValveType { get; set; }
        public string Name { get; set; }
        public List<string> SerialNumbers { get; set; }

        public List<ValveProperty> Properties { get; private set; } = new List<ValveProperty>();

        public bool Equals(Valve other)
        {
            if (other == null) return false;
            if (other.Id == Id) return true;
            else return false;
        }

        public override bool Equals(object obj) => Equals(obj as Valve);
        public override int GetHashCode() => Id.GetHashCode();
    }
}
