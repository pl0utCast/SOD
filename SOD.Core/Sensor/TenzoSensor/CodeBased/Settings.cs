using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOD.Core.Settings;
using System.Collections.ObjectModel;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;

namespace SOD.Core.Sensor.TenzoSensor.CodeBased
{
    [SensorSettings]
    public class Settings
    {
        public Settings()
        {
            Unit = ForceUnit.KilogramForce;
        }
        public string Name { get; set; }
        public int ChannelId { get; set; }
        public ForceUnit Unit { get; set; } = ForceUnit.KilogramForce;
        public string Accaury { get; set; } = "F2";
        public string SensorHint { get; set; } = "";
        public ObservableCollection<CoefficientItem> Coefficients { get; set; } = new ObservableCollection<CoefficientItem>();
    }

    public class CoefficientItem
    {
        public CoefficientItem()
        {

        }

        public int Id { get; set; } = 0;
        public int SavedCode { get; set; }
        public Force InitialValue { get; set; } = Force.FromKilogramsForce(0);
    }
}
