using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnitsNet;

namespace SOD.App.Testing.Funcional
{
    public class Result : ITestingResult
    {
        public bool IsFill => throw new NotImplementedException();

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

        public class PostResult
        {
            public Pressure ExpectedSetPressure { get; set; }
            public Pressure OpenPressure { get; set; }
            public Pressure ClosePressure { get; set; }
            public double Accuracy => Math.Round(((OpenPressure / ExpectedSetPressure) - 1) * 100.0, 1);
            public int OpenPoint { get; set; }
            public int ClosePoint { get; set; }
            public string Result { get; set; }
        }
    }
}
