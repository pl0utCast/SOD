using Newtonsoft.Json;
using SOD.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.CustomUnits
{
    public class DinPressureClass
    {
        public DinPressureClass(DinPressureClassUnit dinPressureClassUnit)
        {
            switch (dinPressureClassUnit)
            {
                case DinPressureClassUnit.PN1:
                    Value = new Pressure(1, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN1_6:
                    Value = new Pressure(1.6, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN2_5:
                    Value = new Pressure(2.5, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN4:
                    Value = new Pressure(4, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN6:
                    Value = new Pressure(6, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN10:
                    Value = new Pressure(10, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN16:
                    Value = new Pressure(16, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN25:
                    Value = new Pressure(25, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN40:
                    Value = new Pressure(40, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN64:
                    Value = new Pressure(64, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN100:
                    Value = new Pressure(100, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN160:
                    Value = new Pressure(160, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN250:
                    Value = new Pressure(250, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN320:
                    Value = new Pressure(320, UnitsNet.Units.PressureUnit.Bar);
                    break;
                case DinPressureClassUnit.PN400:
                    Value = new Pressure(400, UnitsNet.Units.PressureUnit.Bar);
                    break;
                default:
                    break;
            }
            Unit = dinPressureClassUnit;
        }
        // по умолчанию минимальный DN
        public DinPressureClass()
        {
            Value = new Pressure(1, UnitsNet.Units.PressureUnit.Bar);
            Unit = DinPressureClassUnit.PN1;
        }
        [JsonProperty]
        public DinPressureClassUnit Unit { get; private set; }
        [JsonProperty]
        public Pressure Value { get; private set; }

        public override string ToString()
        {
            return new UnitTypeInfo(Unit).Name;
        }
    }
}
