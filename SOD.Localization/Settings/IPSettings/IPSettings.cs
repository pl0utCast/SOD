using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.IPSettings
{
    public class IPSettings
    {
        public string EthernetNames { get; set; } = "Сетевые подключения";
        public string IpAddress { get; set; } = "IP-адрес";
        public string Submask { get; set; } = "Маска подсети";
        public string Dns { get; set; } = "Основной шлюз";
        public string UseDHCP { get; set; } = "Использовать DHCP";
        public string WrongIpMask { get; set; } = "Неправильные Ip-адрес или маска";
        public string WrongDNS { get; set; } = "Неправильный DNS";
        public string SaveIp { get; set; } = "Сетевые настройки успешно сохранены";
    }
}
