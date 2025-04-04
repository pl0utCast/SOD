using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Localization.Testing.SODBench
{
    public class ServiceParameters
    {
        public string ServiceParams { get; set; } = "СЕРВИСНЫЕ ПАРАМЕТРЫ";
        public string PressureVelocity { get; set; } = "Скорость набора давления";
        public string HydraulicPressureCoef { get; set; } = "Коэффициент мультипликации гидро";
        public string MaxAllowablePressure { get; set; } = "Допустимое значение датчика давления, бар";
        public string ThrottleActivationPercentage { get; set; } = "% включения дросселирования";
        public string ThrottleDeactivationPercentage { get; set; } = "% отключения дросселирования";
        public string OverpressureAllowancePercentage { get; set; } = "% превыш. макс. допуст. давления";
        public string NominalPressureValue { get; set; } = "Номин. значение датчика давления, бар";
        public string Reserve1 { get; set; } = "Резерв 2";
        public string Reserve2 { get; set; } = "Резерв 3";
        public string VesselEmergencyLevel_5kg { get; set; } = "Аварийный уровень в колбе 5 кг";
        public string VesselEmergencyLevel_10kg { get; set; } = "Аварийный уровень в колбе 10 кг";
        public string VesselEmergencyLevel_30kg { get; set; } = "Аварийный уровень в колбе 30 кг";
        public string KpPID { get; set; } = "Kp ПИД реуглятора КР1";
        public string KiPID { get; set; } = "Ki ПИД регулятора КР2";
        public string KdPID { get; set; } = "Kd ПИД регулятора КР1";
        public string dwePID { get; set; } = "dwe ПИД регулятор КР1";
        public string tfPID { get; set; } = "tf ПИД регулятор КР1";
        public string MaxOutputConst { get; set; } = "Максимальная выходная константа";
        public string ErrorZonePID { get; set; } = "Зона ошибок ПИД регулятора КР1";
        public string MinOutputConst { get; set; } = "Минимальная выходная константа";
        public string CyclePID { get; set; } = "Цикл ПИД регулятора КР1";
    }
}
