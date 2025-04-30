using SOD.Dialogs;
using SOD.LocalizationService;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SciChart.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace SOD.ViewModels.Settings.IPSettings
{
    public class IPSettingsViewModel : ReactiveObject
    {
        private readonly IDialogService _dialogService;
        private readonly ILocalizationService _localizationService;
        private string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

        public IPSettingsViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            ILocalizationService localizationService)
        {
            ViewTitle = localizationService["Settings.ConfigIpAddress"];
            _dialogService = dialogService;
            _localizationService = localizationService;

            PropertyChanged += OnViewModelPropertyChanged;

            Update = ReactiveCommand.Create(() =>
            {
                // Получаем список всех включенных сетевых соединений
                EthernetNamesList = NetworkInterface.GetAllNetworkInterfaces().Select(x => x.Name).ToList();
                if (EthernetNamesList.Count > 0)
                    EthernetName = EthernetNamesList.FirstOrDefault();

                UpdateIpMask();
            });

            Update.Execute().Subscribe();

            Apply = ReactiveCommand.Create(() =>
            {
                // Если выбрали автоматический Ip-адрес
                if (UseDHCP)
                {
                    SetIP("/c netsh interface ipv4 set address \"" + EthernetName + "\" dhcp & netsh interface ipv4 set dns \"" + EthernetName + "\" dhcp");
                }
                else
                {
                    // Если ip и маски выставлены корректно
                    if (Regex.IsMatch(IpAddress, pattern) && Regex.IsMatch(Subnet, pattern))
                    {
                        // Без основного шлюза
                        if (EthernetDns.IsNullOrEmpty())
                        {
                            SetIP("/c netsh interface ipv4 set address \"" + EthernetName + "\" static " + IpAddress
                                + " " + Subnet + " & netsh interface ipv4 set dns \"" + EthernetName + "\" dhcp");
                        }
                        else
                        {
                            // Если основной шлюз выставлен корректно
                            if (Regex.IsMatch(EthernetDns, pattern))
                            {
                                SetIP("/c netsh interface ipv4 set address \"" + EthernetName + "\" static " + IpAddress + " " + Subnet
                                    + " " + EthernetDns + " & netsh interface ipv4 set dns \"" + EthernetName + "\" static " + EthernetDns);
                            }
                            else
                            {
                                _dialogService.ShowMessage(_localizationService["Settings.IPSettings.WrongDNS"]);
                            }
                        }
                    }
                    else
                    {
                        _dialogService.ShowMessage(_localizationService["Settings.IPSettings.WrongIpMask"]);
                    }
                }
            });

            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
        }

        public void UpdateIpMask()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //Debug.WriteLine(ni.Name);
                    if (ni.GetIPProperties().UnicastAddresses.Count > 1)
                    {
                        var ip = ni.GetIPProperties().UnicastAddresses.LastOrDefault();
                        var gates = ni.GetIPProperties().GatewayAddresses;

                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && ni.Name == EthernetName)
                        {
                            IpAddress = ip.Address.ToString();
                            Subnet = ip.IPv4Mask.ToString();

                            if (gates.Count > 0)
                                EthernetDns = gates.LastOrDefault().Address.ToString();
                            else
                                EthernetDns = null;

                            UseDHCP = false;
                        }
                    }
                    else
                    {
                        var ip = ni.GetIPProperties().UnicastAddresses.FirstOrDefault();
                        var gates = ni.GetIPProperties().GatewayAddresses;

                        if (ip?.Address.AddressFamily == AddressFamily.InterNetwork && ni.Name == EthernetName)
                        {
                            IpAddress = ip.Address.ToString();
                            Subnet = ip.IPv4Mask.ToString();

                            if (gates.Count > 0)
                                EthernetDns = gates.LastOrDefault().Address.ToString();
                            else
                                EthernetDns = null;

                            UseDHCP = true;
                        }
                    }
                }
            }
        }

        private void SetIP(string arg)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                psi.UseShellExecute = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.Verb = "runas";
                psi.Arguments = arg;
                Process.Start(psi);

                _dialogService.ShowMessage(_localizationService["Settings.IPSettings.SaveIp"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EthernetName))
                UpdateIpMask();
        }

        public ReactiveCommand<Unit, Unit> Update { get; set; }
        public ReactiveCommand<Unit, Unit> Apply { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }

        [Reactive]
        public List<string> EthernetNamesList { get; set; } = new List<string>();
        [Reactive]
        public string EthernetName { get; set; }
        [Reactive]
        public string IpAddress { get; set; }
        [Reactive]
        public string Subnet { get; set; }
        [Reactive]
        public string EthernetDns { get; set; }
        [Reactive]
        public bool UseDHCP { get; set; }
        public string ViewTitle { get; set; } = "Настройка IP-адреса";
    }
}
