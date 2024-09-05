using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.CustomUnits
{
    public static class CustomUnitAbbrevaitions
    {
        public static string GetDefaultAbbrevation(Type type, int value)
        {
            if (type == typeof(DinPressureClassUnit))
            {
                var unit = (DinPressureClassUnit)Enum.ToObject(type, value);
                switch (unit)
                {
                    case DinPressureClassUnit.PN1:
                        return "1";
                    case DinPressureClassUnit.PN1_6:
                        return "1.6";
                    case DinPressureClassUnit.PN2_5:
                        return "2.5";
                    case DinPressureClassUnit.PN4:
                        return "4";
                    case DinPressureClassUnit.PN6:
                        return "6";
                    case DinPressureClassUnit.PN10:
                        return "10";
                    case DinPressureClassUnit.PN16:
                        return "16";
                    case DinPressureClassUnit.PN25:
                        return "25";
                    case DinPressureClassUnit.PN40:
                        return "40";
                    case DinPressureClassUnit.PN64:
                        return "64";
                    case DinPressureClassUnit.PN100:
                        return "100";
                    case DinPressureClassUnit.PN160:
                        return "160";
                    case DinPressureClassUnit.PN250:
                        return "250";
                    case DinPressureClassUnit.PN320:
                        return "320";
                    case DinPressureClassUnit.PN400:
                        return "400";
                    default:
                        break;
                }
            }
            else if (type == typeof(AnsiPressureClassUnit))
            {
                var unit = (AnsiPressureClassUnit)Enum.ToObject(type, value);
                switch (unit)
                {
                    case AnsiPressureClassUnit.CLASS150:
                        return "CLASS 150";
                    case AnsiPressureClassUnit.CLASS300:
                        return "CLASS 300";
                    case AnsiPressureClassUnit.CLASS400:
                        return "CLASS 400";
                    case AnsiPressureClassUnit.CLASS600:
                        return "CLASS 600";
                    case AnsiPressureClassUnit.CLASS900:
                        return "CLASS 900";
                    case AnsiPressureClassUnit.CLASS1500:
                        return "CLASS 1500";
                    case AnsiPressureClassUnit.CLASS2500:
                        return "CLASS 2500";
                    default:
                        break;
                }
            }
            else if (type == typeof(NominalPipeSizeMetricUnit))
            {
                var unit = (NominalPipeSizeMetricUnit)Enum.ToObject(type, value);
                switch (unit)
                {
                    case NominalPipeSizeMetricUnit.DN10:
                        return "10";
                    case NominalPipeSizeMetricUnit.DN15:
                        return "15";
                    case NominalPipeSizeMetricUnit.DN20:
                        return "20";
                    case NominalPipeSizeMetricUnit.DN25:
                        return "25";
                    case NominalPipeSizeMetricUnit.DN32:
                        return "32";
                    case NominalPipeSizeMetricUnit.DN40:
                        return "40";
                    case NominalPipeSizeMetricUnit.DN50:
                        return "50";
                    case NominalPipeSizeMetricUnit.DN65:
                        return "65";
                    case NominalPipeSizeMetricUnit.DN80:
                        return "80";
                    case NominalPipeSizeMetricUnit.DN100:
                        return "100";
                    case NominalPipeSizeMetricUnit.DN125:
                        return "125";
                    case NominalPipeSizeMetricUnit.DN150:
                        return "150";
                    case NominalPipeSizeMetricUnit.DN200:
                        return "200";
                    case NominalPipeSizeMetricUnit.DN250:
                        return "250";
                    case NominalPipeSizeMetricUnit.DN300:
                        return "300";
                    case NominalPipeSizeMetricUnit.DN350:
                        return "350";
                    case NominalPipeSizeMetricUnit.DN400:
                        return "400";
                    case NominalPipeSizeMetricUnit.DN450:
                        return "450";
                    case NominalPipeSizeMetricUnit.DN500:
                        return "500";
                    case NominalPipeSizeMetricUnit.DN600:
                        return "600";
                    case NominalPipeSizeMetricUnit.DN700:
                        return "700";
                    case NominalPipeSizeMetricUnit.DN800:
                        return "800";
                    case NominalPipeSizeMetricUnit.DN900:
                        return "900";
                    case NominalPipeSizeMetricUnit.DN1000:
                        return "1000";
                    case NominalPipeSizeMetricUnit.DN1200:
                        return "1200";
                    case NominalPipeSizeMetricUnit.DN3:
                        return "3";
                    case NominalPipeSizeMetricUnit.DN6:
                        return "6";
                    case NominalPipeSizeMetricUnit.DN650:
                        return "650";
                    case NominalPipeSizeMetricUnit.DN750:
                        return "750";
                    case NominalPipeSizeMetricUnit.DN1050:
                        return "1050";
                    case NominalPipeSizeMetricUnit.DN1400:
                        return "1400";
                    case NominalPipeSizeMetricUnit.DN1600:
                        return "1600";
                    case NominalPipeSizeMetricUnit.DN1800:
                        return "1800";
                    case NominalPipeSizeMetricUnit.DN2000:
                        return "2000";
                    case NominalPipeSizeMetricUnit.DN2200:
                        return "2200";
                    case NominalPipeSizeMetricUnit.DN2400:
                        return "2400";
                    default:
                        break;
                }
            }
            else if (type == typeof(NominalPipeSizeInchUnit))
            {
                var unit = (NominalPipeSizeInchUnit)Enum.ToObject(type, value);
                switch (unit)
                {
                    case NominalPipeSizeInchUnit.INCH3_8:
                        return "3/8";
                    case NominalPipeSizeInchUnit.INCH1_2:
                        return "1/2";
                    case NominalPipeSizeInchUnit.INCH3_4:
                        return "3/4";
                    case NominalPipeSizeInchUnit.INCH1:
                        return "1";
                    case NominalPipeSizeInchUnit.INCH1_1_4:
                        return "1 1/4";
                    case NominalPipeSizeInchUnit.INCH1_1_2:
                        return "1 1/2";
                    case NominalPipeSizeInchUnit.INCH2:
                        return "2";
                    case NominalPipeSizeInchUnit.INCH2_1_2:
                        return "2 1/2";
                    case NominalPipeSizeInchUnit.INCH3:
                        return "3";
                    case NominalPipeSizeInchUnit.INCH4:
                        return "4";
                    case NominalPipeSizeInchUnit.INCH5:
                        return "5";
                    case NominalPipeSizeInchUnit.INCH6:
                        return "6";
                    case NominalPipeSizeInchUnit.INCH8:
                        return "8";
                    case NominalPipeSizeInchUnit.INCH10:
                        return "10";
                    case NominalPipeSizeInchUnit.INCH12:
                        return "12";
                    case NominalPipeSizeInchUnit.INCH14:
                        return "14";
                    case NominalPipeSizeInchUnit.INCH16:
                        return "16";
                    case NominalPipeSizeInchUnit.INCH18:
                        return "18";
                    case NominalPipeSizeInchUnit.INCH20:
                        return "20";
                    case NominalPipeSizeInchUnit.INCH24:
                        return "24";
                    case NominalPipeSizeInchUnit.INCH28:
                        return "28";
                    case NominalPipeSizeInchUnit.INCH32:
                        return "32";
                    case NominalPipeSizeInchUnit.INCH36:
                        return "36";
                    case NominalPipeSizeInchUnit.INCH40:
                        return "40";
                    case NominalPipeSizeInchUnit.INCH48:
                        return "48";
                    case NominalPipeSizeInchUnit.INCH1_8:
                        return "1/8";
                    case NominalPipeSizeInchUnit.INCH1_4:
                        return "1/4";
                    case NominalPipeSizeInchUnit.INCH26:
                        return "26";
                    case NominalPipeSizeInchUnit.INCH30:
                        return "30";
                    case NominalPipeSizeInchUnit.INCH42:
                        return "42";
                    case NominalPipeSizeInchUnit.INCH56:
                        return "56";
                    case NominalPipeSizeInchUnit.INCH64:
                        return "64";
                    case NominalPipeSizeInchUnit.INCH72:
                        return "72";
                    case NominalPipeSizeInchUnit.INCH80:
                        return "80";
                    case NominalPipeSizeInchUnit.INCH88:
                        return "88";
                    case NominalPipeSizeInchUnit.INCH96:
                        return "96";
                    default:
                        break;
                }
            }
            throw new ArgumentException(nameof(type));
        }
    }
}
