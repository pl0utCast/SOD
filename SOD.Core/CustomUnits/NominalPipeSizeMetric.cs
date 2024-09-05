using Newtonsoft.Json;
using SOD.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.CustomUnits
{
    public class NominalPipeSizeMetric
    {
        public NominalPipeSizeMetric(NominalPipeSizeMetricUnit nominalPipeSizeMetricUnit)
        {
            Unit = nominalPipeSizeMetricUnit;
            switch (nominalPipeSizeMetricUnit)
            {
                case NominalPipeSizeMetricUnit.DN10:
                    Value = new Length(10, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN15:
                    Value = new Length(15, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN20:
                    Value = new Length(20, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN25:
                    Value = new Length(25, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN32:
                    Value = new Length(32, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN40:
                    Value = new Length(40, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN50:
                    Value = new Length(50, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN65:
                    Value = new Length(65, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN80:
                    Value = new Length(80, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN100:
                    Value = new Length(100, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN125:
                    Value = new Length(125, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN150:
                    Value = new Length(150, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN200:
                    Value = new Length(200, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN250:
                    Value = new Length(250, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN300:
                    Value = new Length(300, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN350:
                    Value = new Length(350, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN400:
                    Value = new Length(400, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN450:
                    Value = new Length(450, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN500:
                    Value = new Length(500, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN600:
                    Value = new Length(600, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN700:
                    Value = new Length(700, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN800:
                    Value = new Length(800, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN900:
                    Value = new Length(900, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1000:
                    Value = new Length(1000, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1200:
                    Value = new Length(1200, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN3:
                    Value = new Length(3, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN6:
                    Value = new Length(6, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN650:
                    Value = new Length(650, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN750:
                    Value = new Length(750, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1050:
                    Value = new Length(1050, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1400:
                    Value = new Length(1400, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1600:
                    Value = new Length(1600, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN1800:
                    Value = new Length(1800, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN2000:
                    Value = new Length(2000, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN2200:
                    Value = new Length(2200, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                case NominalPipeSizeMetricUnit.DN2400:
                    Value = new Length(2400, UnitsNet.Units.LengthUnit.Millimeter);
                    break;
                default:
                    break;
            }
        }
        public NominalPipeSizeMetric()
        {
            Value = new Length(10, UnitsNet.Units.LengthUnit.Millimeter);
            Unit = NominalPipeSizeMetricUnit.DN10;
        }
        [JsonProperty]
        public NominalPipeSizeMetricUnit Unit { get; private set; }
        [JsonProperty]
        public Length Value { get; private set; }

        public override string ToString()
        {
            return new UnitTypeInfo(Unit).Name;
        }
    }
}
