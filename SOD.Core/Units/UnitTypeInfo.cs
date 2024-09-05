using SOD.Core.CustomUnits;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.Core.Units
{
    public class UnitTypeInfo
    {
        public UnitTypeInfo(object unitType)
        {
            if (unitType.GetType() == typeof(DinPressureClassUnit))
            {
                Name = CustomUnitAbbrevaitions.GetDefaultAbbrevation(unitType.GetType(), (int)unitType);
            }
            else if (unitType.GetType() == typeof(AnsiPressureClassUnit))
            {
                Name = CustomUnitAbbrevaitions.GetDefaultAbbrevation(unitType.GetType(), (int)unitType);
            }
            else if (unitType.GetType()==typeof(NominalPipeSizeMetricUnit))
            {
                Name = CustomUnitAbbrevaitions.GetDefaultAbbrevation(unitType.GetType(), (int)unitType);
            }
            else if (unitType.GetType() == typeof(NominalPipeSizeInchUnit))
            {
                Name = CustomUnitAbbrevaitions.GetDefaultAbbrevation(unitType.GetType(), (int)unitType);
            }
            else
            {
                Name = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(unitType.GetType(), (int)unitType);
            }
            UnitType = (Enum)unitType;
        }
        
        public string Name { get; }
        public Enum UnitType { get; }
    }
}
