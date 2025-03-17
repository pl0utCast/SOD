using UnitsNet.Units;

namespace SOD.Core.Units
{
	public static class TenzoUnits
	{
        public static IReadOnlyList<Enum> Units => new List<Enum>()
        {
            ForceUnit.Kilonewton,
            ForceUnit.Newton,
            ForceUnit.Micronewton,
            ForceUnit.TonneForce,
            ForceUnit.KilogramForce
        };
    }
}
