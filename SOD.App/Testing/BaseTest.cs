using SOD.App.Benches;
using SOD.App.Mediums;
using SOD.App.Testing.Standarts;
using SOD.Core.Sensor;
using SOD.Core.Valves;
using SOD.LocalizationService;
using System.Drawing;
using System.Reactive.Disposables;
using UnitsNet;

namespace SOD.App.Testing
{
    public abstract  class BaseTest //: ITesting, IDisposable
    {
        protected IEnumerable<IPost> _posts;
        protected Dictionary<int, List<Pressure>> pressures = new Dictionary<int,List<Pressure>>();
        protected Dictionary<int, List<VolumeFlow>> leakages = new Dictionary<int, List<VolumeFlow>>();
        //private List<IDisposable> disposables = new List<IDisposable>();
        CompositeDisposable disposables = new CompositeDisposable();
        protected List<int> registrationMarkers { get; set; } = new List<int>();

        protected readonly ILocalizationService localizationService;
        protected readonly object[] _parameters;
        protected Valve _valve;
        protected MediumType _mediumType;

        private ITestBench _testBench;

        private Timer timer;
        private List<Action> sensorValueUpdaters;
        public BaseTest(ILocalizationService localizationService, IStandart standart=null, params object[] parameters)
        {
            this.localizationService = localizationService;
            _parameters = parameters;
        }

        public void Start(ITestBench testBench, MediumType mediumType)
        {
            IsRun = true;
            //_posts = testBench.Posts;
            //_valve = testBench.TestingValve;
            _testBench = testBench;
            _mediumType = mediumType;
            registrationMarkers.Clear();
            pressures.Clear();
            leakages.Clear();
        }

        public void Stop()
        {
            IsRun = false;
            timer?.Dispose();
            disposables.Dispose();
        }

        public abstract void CalculateResult();
        public abstract void FillReport(Bitmap chartImage = null);
        public abstract void Dispose();
        public abstract void StartRegistration();
        public abstract void StopRegistration();

        public object[] GetTestParameters() => _parameters;

        public void StartCollectData()
        {
            sensorValueUpdaters = new List<Action>();
            foreach (var post in _posts)
            {
                foreach (var sensor in post.Sensors)
                {
                    if (sensor.Sensor is IPressureSensor pressureSensor)
                    {
                        pressures.Add(pressureSensor.Id, new List<Pressure>());
                        sensorValueUpdaters.Add(() =>
                        {
                            pressures[pressureSensor.Id].Add(pressureSensor.Pressure.ToUnit(_testBench.Settings.PressureUnit));
                        });
                        /*pressureSensor.Subscribe(p =>
                        {
                            var press = p.ToUnit(_testBench.Settings.PressureUnit);
                            pressures[pressureSensor.Id].Add(press);
                        }).DisposeWith(disposables);*/
                    }
                    //if (sensor.Sensor is ILeakageSensor leakageSensor)
                    //{
                    //    leakageSensor.Reset();
                    //    leakages.Add(leakageSensor.Id, new List<VolumeFlow>());
                    //    sensorValueUpdaters.Add(() =>
                    //    {
                    //        leakages[leakageSensor.Id].Add(leakageSensor.Flow.ToUnit(_testBench.Settings.LeakageUnit));
                    //    });
                        /*leakageSensor.Subscribe(l =>
                        {
                            var leak = l.ToUnit(_testBench.Settings.LeakageUnit);
                            leakages[leakageSensor.Id].Add(leak);
                        }).DisposeWith(disposables);*/
                    //}
                }
            }

            timer?.Dispose();
            // таймер тикает каждые 100 мс
            timer = new Timer(c =>
            {
                foreach (var updater in sensorValueUpdaters)
                {
                    updater();
                }
            }, null, 0, 100);

            
        }

        public string Name { get; set; }

        public abstract TestType TestType { get; }
        public bool IsRun { get; protected set; }

        public abstract ITestingResult Result { get; }
    }
}
