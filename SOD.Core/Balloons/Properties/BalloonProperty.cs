using LiteDB;
using SOD.Core.Props;

namespace SOD.Core.Balloons.Properties
{
    public class BalloonProperty : IEquatable<BalloonProperty>
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        [BsonIgnore]
        public object Value { get; set; }
        public PropertyType Type { get; set; }
        public string SerializedValue { get; set; }

        public bool Equals(BalloonProperty other)
        {
            if (other == null) return false;
            if (Id == other.Id) return true;
            else return false;
        }

        public override bool Equals(object obj) => Equals(obj as BalloonProperty);
        public override int GetHashCode() => Id.GetHashCode();
    }
}
