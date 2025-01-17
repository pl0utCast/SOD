using SOD.Core.Balloons;

namespace SOD.App.Benches
{
    public interface ITestBench
    {
        //public BenchesType Type { get; }
        public IEnumerable<IPost> Posts { get; }
        public IBenchSettings Settings { get; }
        public Balloon TestingBalloon { get; set; }
        //public Valve TestingValve { get; set; }
    }
}
