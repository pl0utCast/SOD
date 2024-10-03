using SOD.Core.CustomUnits;
using SOD.Core.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace SOD.Core.Balloons.Properties
{
	public static class PropertyHelper
	{
		public static string GetPropertyTypeName(this PropertyType propertyType)
		{
			switch (propertyType)
			{
				case PropertyType.Pressure:
					return "Давление";
				case PropertyType.VolumeFlow:
					return "Расход";
				case PropertyType.Volume:
					return "Объём";
				case PropertyType.Area:
					return "Площадь";
				case PropertyType.Lenght:
					return "Длина";
				case PropertyType.PNTable:
					return "Таблица PN";
				case PropertyType.AnsiClassTable:
					return "Таблица ANSI CLASS";
				case PropertyType.String:
					return "Текст";
				case PropertyType.Double:
					return "Дробное число";
				case PropertyType.Integer:
					return "Целое число";
				case PropertyType.NPSMetric:
					return "NPS мм";
				case PropertyType.NPSInch:
					return "NPS Inch";
				case PropertyType.StringList:
					return "Список";
				case PropertyType.SealType:
					return "Уплотнение";
				case PropertyType.TestMedium:
					return "Тестовая среда";
				case PropertyType.BalloonType:
					return "Тип баллона";
				default: return Enum.GetName(typeof(PropertyType), propertyType);
			}
		}
		public static Dictionary<string, PropertyType> GetPropertyTypeDictonary()
		{
			var result = new Dictionary<string, PropertyType>();
			foreach (var PropType in Enum.GetValues(typeof(PropertyType)))
			{
				if (PropType is PropertyType propertyType)
				{
					result.Add(propertyType.GetPropertyTypeName(), propertyType);
				}
			}
			return result;
		}

		public static object GetDefaultValueByType(this PropertyType propertyType)
		{
			switch (propertyType)
			{
				case PropertyType.Pressure:
					return new Pressure(0, UnitsNet.Units.PressureUnit.Bar);
				case PropertyType.VolumeFlow:
					return new VolumeFlow(0, UnitsNet.Units.VolumeFlowUnit.LiterPerMinute);
				case PropertyType.Volume:
					return new Volume(0, UnitsNet.Units.VolumeUnit.CubicCentimeter);
				case PropertyType.Area:
					return new Area(0, UnitsNet.Units.AreaUnit.SquareMillimeter);
				case PropertyType.Lenght:
					return new Length(0, UnitsNet.Units.LengthUnit.Millimeter);
				case PropertyType.PNTable:
					return new DinPressureClass();
				case PropertyType.AnsiClassTable:
					return new AnsiPressureClass();
				case PropertyType.String:
					return string.Empty;
				case PropertyType.Double:
					return 0.0;
				case PropertyType.Integer:
					return 0;
				case PropertyType.NPSMetric:
					return new NominalPipeSizeMetric();
				case PropertyType.NPSInch:
					return new NominalPipeSizeInch();
				case PropertyType.StringList:
					return new List<object>();
				case PropertyType.SealType:
					return Core.Seals.SealType.Metal;
				case PropertyType.TestMedium:
					return Core.Seals.TestMediumType.Water;
				case PropertyType.BalloonType:
					return BalloonType.KPG1;
				default: return null;
			}
		}


	}
}
