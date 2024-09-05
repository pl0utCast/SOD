using Newtonsoft.Json;
using SOD.Core.Units;
using UnitsNet;

namespace SOD.Core.CustomUnits
{
    public class NominalPipeSizeInch
    {
        public NominalPipeSizeInch(NominalPipeSizeInchUnit nominalPipeSizeInchUnit)
        {
            Unit = nominalPipeSizeInchUnit;
            switch (nominalPipeSizeInchUnit)
            {
                case NominalPipeSizeInchUnit.INCH3_8:
                    Value = new Length(0.375, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1_2:
                    Value = new Length(0.5, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH3_4:
                    Value = new Length(0.75, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1:
                    Value = new Length(1, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1_1_4:
                    Value = new Length(1.25, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1_1_2:
                    Value = new Length(1.5, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH2:
                    Value = new Length(2, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH2_1_2:
                    Value = new Length(2.5, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH3:
                    Value = new Length(3, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH4:
                    Value = new Length(4, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH5:
                    Value = new Length(5, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH6:
                    Value = new Length(6, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH8:
                    Value = new Length(8, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH10:
                    Value = new Length(10, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH12:
                    Value = new Length(12, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH14:
                    Value = new Length(14, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH16:
                    Value = new Length(16, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH18:
                    Value = new Length(18, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH20:
                    Value = new Length(20, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH24:
                    Value = new Length(24, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH28:
                    Value = new Length(28, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH32:
                    Value = new Length(32, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH36:
                    Value = new Length(36, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH40:
                    Value = new Length(40, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH48:
                    Value = new Length(48, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1_8:
                    Value = new Length(1.0 / 8.0, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH1_4:
                    Value = new Length(1.0 / 4.0, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH26:
                    Value = new Length(26, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH30:
                    Value = new Length(30, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH42:
                    Value = new Length(42, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH56:
                    Value = new Length(56, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH64:
                    Value = new Length(64, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH72:
                    Value = new Length(72, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH80:
                    Value = new Length(80, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH88:
                    Value = new Length(88, UnitsNet.Units.LengthUnit.Inch);
                    break;
                case NominalPipeSizeInchUnit.INCH96:
                    Value = new Length(96, UnitsNet.Units.LengthUnit.Inch);
                    break;
                default:
                    break;
            }
        }
        public NominalPipeSizeInch()
        {
            Value = new Length(0.375, UnitsNet.Units.LengthUnit.Inch);
            Unit = NominalPipeSizeInchUnit.INCH3_8;
        }
        [JsonProperty]
        public NominalPipeSizeInchUnit Unit { get; private set; }
        [JsonProperty]
        public Length Value { get; set; }

        public override string ToString()
        {
            return new UnitTypeInfo(Unit).Name;
        }
    }
}
