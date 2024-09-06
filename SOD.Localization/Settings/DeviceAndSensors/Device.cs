using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.DeviceAndSensors
{
    public class Device
    {
        public string SaveSettings { get; set; } = "Сохранить настройки?";
        public string Yes { get; set; } = "Да";
        public string No { get; set; } = "Нет";
        public string Name { get; set; } = "Название";
        public string Channels { get; set; } = "Каналы постоянного опроса";
        public string IpAddress { get; set; } = "IP адрес";
        public string Port { get; set; } = "Порт";
        public string RefreshRegisters { get; set; } = "Регистры постоянного опроса";
        public string RegistersReadWrite { get; set; } = "Регистры чтения/записи";
        public string Registers { get; set; } = "Регистры";
        public string UseDigitalSensors { get; set; } = "Использовать цифровые датчики";
        public string BaudRate { get; set; } = "Скорость";
        public string Address { get; set; } = "Адрес:";
        public string Value { get; set; } = "Значение:";

    }
}
