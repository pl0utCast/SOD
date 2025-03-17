using SOD.Core.CustomUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.Core.Units
{
    public static class UnitsHelper
    {

        public static string GetUnitName(this IQuantity quantity)
        {
            object unit = quantity.Unit;
            return UnitAbbreviationsCache.Default.GetDefaultAbbreviation(quantity.Unit.GetType(),(int)unit);
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this Enum unitType)
        {
            if (PressureUnits.Units.Contains(unitType)) return PressureUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (VolumeFlowUnits.Units.Contains(unitType)) return VolumeFlowUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (VolumeUnits.Units.Contains(unitType)) return VolumeUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (AreaUnits.Units.Contains(unitType)) return AreaUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (LengthUnits.Units.Contains(unitType)) return LengthUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (TemperatureUnits.Units.Contains(unitType)) return TemperatureUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
            if (TenzoUnits.Units.Contains(unitType)) return TenzoUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();

			return null;
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this IQuantity quantity)
        {
            switch (quantity.QuantityInfo.Name)
            {
                case nameof(Pressure): return PressureUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
                case nameof(VolumeFlow): return VolumeFlowUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
                case nameof(Volume): return VolumeUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
                case nameof(Area): return AreaUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
                case nameof(Length): return LengthUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
                case nameof(Temperature): return TemperatureUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
				case nameof(Force): return TenzoUnits.Units.Select(u => new UnitTypeInfo(u)).ToList();
				default:return null;
            }
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this DinPressureClass dinPressureClass)
        {
            var list = new List<UnitTypeInfo>();
            foreach (var pn in Enum.GetValues(typeof(DinPressureClassUnit)))
            {
                list.Add(new UnitTypeInfo(pn));
            }
            return list;
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this AnsiPressureClass dinPressureClass)
        {
            var list = new List<UnitTypeInfo>();
            foreach (var pn in Enum.GetValues(typeof(AnsiPressureClassUnit)))
            {
                list.Add(new UnitTypeInfo(pn));
            }
            return list;
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this NominalPipeSizeMetric nominalPipeSizeMetric)
        {
            var list = new List<UnitTypeInfo>();
            foreach (var nps in Enum.GetValues(typeof(NominalPipeSizeMetricUnit)))
            {
                list.Add(new UnitTypeInfo(nps));
            }
            return list;
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(this NominalPipeSizeInch nominalPipeSizeInch)
        {
            var list = new List<UnitTypeInfo>();
            foreach (var nps in Enum.GetValues(typeof(NominalPipeSizeInchUnit)))
            {
                list.Add(new UnitTypeInfo(nps));
            }
            return list;
        }

        public static IReadOnlyList<UnitTypeInfo> GetUnitTypeInfo(Type type, object customUnit)
        {
            if (typeof(AnsiPressureClass) == type) return GetUnitTypeInfo((AnsiPressureClass)customUnit);
            if (typeof(DinPressureClass) == type) return GetUnitTypeInfo((DinPressureClass)customUnit);
            if (typeof(NominalPipeSizeMetric) == type) return GetUnitTypeInfo((NominalPipeSizeMetric)customUnit);
            if (typeof(NominalPipeSizeInch) == type) return GetUnitTypeInfo((NominalPipeSizeInch)customUnit);
            return null;
        }

        public static object GetValue(double value, UnitTypeInfo unitTypeInfo)
        {
            var type = default(Type);
            switch (unitTypeInfo.UnitType.GetType().Name)
            {
                case  nameof(PressureUnit):
                    type = typeof(Pressure);
                    break;
                case nameof(VolumeUnit):
                    type = typeof(Volume);
                    break;
                case nameof(AreaUnit):
                    type = typeof(Area);
                    break;
                case nameof(LengthUnit):
                    type = typeof(Length);
                    break;
                case nameof(VolumeFlowUnit):
                    type = typeof(VolumeFlow);
                    break;
                case nameof(TemperatureUnit):
                    type = typeof(Temperature);
                    break;
				case nameof(MassUnit):
					type = typeof(Mass);
					break;
                case nameof(ForceUnit):
                    type = typeof(Force);
                    break;
                case nameof(DinPressureClassUnit):
                    return new DinPressureClass((DinPressureClassUnit)unitTypeInfo.UnitType);
                case nameof(AnsiPressureClassUnit):
                    return new AnsiPressureClass((AnsiPressureClassUnit)unitTypeInfo.UnitType);
                case nameof(NominalPipeSizeInchUnit):
                    return new NominalPipeSizeInch((NominalPipeSizeInchUnit)unitTypeInfo.UnitType);
                case nameof(NominalPipeSizeMetricUnit):
                    return new NominalPipeSizeMetric((NominalPipeSizeMetricUnit)unitTypeInfo.UnitType);
                default:
                    throw new ArgumentException(nameof(unitTypeInfo));
            }
            var method = type.GetMethod("From", new Type[] { typeof(QuantityValue), unitTypeInfo.UnitType.GetType() });
            var result = method.Invoke(null, new object[] { (QuantityValue)value, unitTypeInfo.UnitType });
            return result;
        }

        public static object GetValue(UnitTypeInfo unitTypeInfo)
        {
            return GetValue(0.0, unitTypeInfo);
        }
    }
}
