using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace SOD.App.Testing.Test
{
	public class Result : ITestingResult
	{
		public void Clear()
		{
			Chart?.Dispose();
			//QrChart?.Dispose();
			PostResults.Clear();
		}
		public Image Chart { get; set; }
		//public Image QrChart { get; set; }
		public List<PostResult> PostResults { get; set; } = new List<PostResult>();
		public string Standart { get; set; }
		//public MediumType Medium { get; set; }

		public bool IsFill => PostResults.Count > 0;

		public class PostResult
		{
			public Pressure ExpectedSetPressure { get; set; }
			public Pressure OpenPressure { get; set; }
			public Pressure ClosePressure { get; set; }
			public double Accuracy => Math.Round(((OpenPressure / ExpectedSetPressure) - 1) * 100.0, 1);
			public int OpenPoint { get; set; }
			public int ClosePoint { get; set; }
			public string Result { get; set; }

			public List<Registration> Registrations { get; set; } = new List<Registration>();
        }

		public class Registration
		{
			public List<SensorResultValue<Pressure>> StartPressure { get; set; } = new List<SensorResultValue<Pressure>>();
			public List<SensorResultValue<Pressure>> StopPressure { get; set; } = new List<SensorResultValue<Pressure>>();
			public List<SensorResultValue<Pressure>> DropPressure { get; set; } = new List<SensorResultValue<Pressure>>();
			//public List<SensorResultValue<VolumeFlow>> Leakage { get; set; } = new List<SensorResultValue<VolumeFlow>>();
			//public List<SensorResultValue<int>> Drops { get; set; } = new List<SensorResultValue<int>>();
			public string Result { get; set; }
			public TimeSpan Time { get; set; }
		}
	}
}
