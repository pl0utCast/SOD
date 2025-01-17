using SOD.Core.Balloons.Properties;

namespace SOD.Core.Balloons
{
    public static class BalloonExtension
    {
        public static BalloonProperty GetBalloonProperty(this Balloon balloon, string prefix)
        {
            return balloon.Properties.SingleOrDefault(vp => vp.Prefix == prefix);
        }

        public static void RemoveProperty(this Balloon balloon, int id)
        {
            var prop = balloon.Properties.SingleOrDefault(p => p.Id == id);
            balloon.Properties.Remove(prop);
        }
    }
}
