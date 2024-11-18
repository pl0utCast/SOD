using System.Text;

namespace SOD.App.Benches.SODBench
{
    public class Post : IPost
    {
        public Post(int id)
        {
            Id = id;
        }
        public int Id { get; set; }

        public bool IsEnable { get; set; }

        public List<BenchSensor> Sensors { get; set; } = new List<BenchSensor>();

        public string SerialNumber { get; set; }
        public PostStatus Status { get; set; }
        IEnumerable<BenchSensor> IPost.Sensors => Sensors;
    }
}
