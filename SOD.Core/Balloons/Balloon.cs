using SOD.Core.Balloons.Properties;

namespace SOD.Core.Balloons
{
    public class Balloon : IEquatable<Balloon>
    {
        public int Id { get; set; }
        public string Name { get; set; }
		public BalloonType BalloonType { get; set; }
		public BalloonTypes BalloonTypes { get; set; }
		public double BalloonVolume { get; set; }
		public int StandartId { get; set; }

        public List<BalloonProperty> Properties { get; private set; } = new List<BalloonProperty>();

        public bool Equals(Balloon other)
        {
            if (other == null) return false;
            if (other.Id == Id) return true;
            else return false;
        }
    }
}
