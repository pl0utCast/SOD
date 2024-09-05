using Newtonsoft.Json;
using SOD.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.CustomUnits
{
    public class AnsiPressureClass
    {
        public AnsiPressureClass()
        {
            Unit = AnsiPressureClassUnit.CLASS150;
            Value = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
        }
        public AnsiPressureClass(AnsiPressureClassUnit ansiPressureClassUnit)
        {
            Unit = ansiPressureClassUnit;
            switch (ansiPressureClassUnit)
            {
                case AnsiPressureClassUnit.CLASS150:
                    Value = new Pressure(20, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS300:
                    Value = new Pressure(50, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS400:
                    Value = new Pressure(64, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS600:
                    Value = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS900:
                    Value = new Pressure(150, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS1500:
                    Value = new Pressure(250, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case AnsiPressureClassUnit.CLASS2500:
                    Value = new Pressure(420, UnitsNet.Units.PressureUnit.Bar);
                    break;
                default:
                    break;
            }
        }
        [JsonProperty]
        public AnsiPressureClassUnit Unit { get; set; }
        [JsonProperty]
        public Pressure Value { get; private set; }

        public override string ToString()
        {
            return new UnitTypeInfo(Unit).Name;
        }
    }
}
