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
    public class TenzoSensorSettings
    {
        public TenzoSensorSettings()
        {
            MinValue = UnitsNet.Force.FromKilogramsForce(0);
            MaxValue = UnitsNet.Force.FromKilogramsForce(100);
            Unit = ForceUnit.KilogramForce;
        }
        public string Name { get; set; }
        public int ChannelId { get; set; }
        public ForceUnit Unit { get; set; } = ForceUnit.KilogramForce;
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Force MinValue { get; set; }
        [JsonConverter(typeof(UnitsNetIQuantityJsonConverter))]
        public UnitsNet.Force MaxValue { get; set; }
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
        public int MinCode { get; set; }
        public Force InitialValue { get; set; } = Force.FromKilogramsForce(0);
    }
}
