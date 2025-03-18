using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.DeviceAndSensors
{
    public class IcpDevice
    {
        public string Add { get; set; } = "Добавить";
        public string Edit { get; set; } = "Редактировать";
        public string Delete { get; set; } = "Удалить";
        public string ModbusId { get; set; } = "Id модбаса";
        public string DataType { get; set; } = "Тип данных";
        public string Value { get; set; } = "Значение";
        public string Description { get; set; } = "Описание";
        public string CoilsRegisters { get; set; } = "Coils регистры";
        public string HoldingRegisters { get; set; } = "Holding регистры";
        public string InputRegisters { get; set; } = "Input регистры";
    }
}
