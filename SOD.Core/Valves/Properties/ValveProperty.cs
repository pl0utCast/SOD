using LiteDB;
using SOD.Core.Props;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Valves.Properties
{
    public class ValveProperty : IEquatable<ValveProperty>
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        [BsonIgnore]
        public object Value { get; set; }
        public PropertyType Type { get; set; }
        public string SerializedValue { get; set; }

        public bool Equals(ValveProperty other)
        {
            if (other == null) return false;
            if (Id == other.Id) return true;
            else return false;
        }

        public override bool Equals(object obj) => Equals(obj as ValveProperty);
        public override int GetHashCode() => Id.GetHashCode();
    }
}
