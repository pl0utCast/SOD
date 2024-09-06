using SOD.App.Mediums;
using System.Drawing;
using UnitsNet;

namespace SOD.App.Testing.Strength
{
    public class Result : ITestingResult
    {
        public void Clear()
        {
            Chart?.Dispose();
            QrChart?.Dispose();
            PostResults.Clear();
        }
        public Image Chart { get; set; }
        public Image QrChart { get; set; }
        public List<PostResult> PostResults { get; set; } = new List<PostResult>();
        public string Standart { get; set; }
        public bool IsFill => PostResults.Count > 0;
        public MediumType Medium { get; set; }

        public class PostResult
        {
            public int PostId { get; set; }
            public string SerialNumber { get; set; }
            public List<Registration> Registrations { get; set; } = new List<Registration>();
        }
        public class Registration
        {
            public List<SensorResultValue<Pressure>> StartPressure { get; set; } = new List<SensorResultValue<Pressure>>();
            public List<SensorResultValue<Pressure>> StopPressure { get; set; } = new List<SensorResultValue<Pressure>>();
            public List<SensorResultValue<Pressure>> DropPressure { get; set; } = new List<SensorResultValue<Pressure>>();
            public string Result { get; set; }
            public TimeSpan Time { get; set; }
        }
    }
}
