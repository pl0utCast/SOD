using SOD.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;

namespace SOD.View
{
    public static class NavigationRegister
    {
        public static void Register(INavigationService navigationService)
        {
            // Testing
            navigationService.RegisterRoot("Testing", "Test", new Testing.TestView(), null);
            //navigationService.RegisterView("Testing", "TestingValves", new Testing.ValvesView(), typeof(ViewModels.Testing.ValvesViewModel));

            navigationService.RegisterView("Testing", "CRSTest", new Testing.CRSBench.TestingView(), typeof(ViewModels.Testing.CRSBench.TestingViewModel));
            navigationService.RegisterView("Testing", "CRSTestParameters", new Testing.CRSBench.TestParametersView(), typeof(ViewModels.Testing.CRSBench.TestParametersViewModel));
            
            // Settings
            navigationService.RegisterRoot("Settings", "SettingsView", new Settings.SettingsView(), typeof(ViewModels.Settings.SettingsViewModel));
            navigationService.RegisterView("Settings", "IPSettingsView", new Settings.IPSettings.IPSettingsView(), typeof(ViewModels.Settings.IPSettings.IPSettingsViewModel));
            navigationService.RegisterView("Settings", "DeviceAndSensorSettingsView", new Settings.DeviceAndSensor.DeviceSensorSettingsView(), typeof(ViewModels.Settings.DeviceAndSensor.DeviceAndSensorSettingsViewModel));
			navigationService.RegisterView("Settings", "BalloonSettings", new Settings.Balloon.BalloonSettingsView(), typeof(ViewModels.Settings.Balloon.BalloonSettingsViewModel));
			navigationService.RegisterView("Settings", "ModbusTcpDeviceSettings", new Settings.DeviceAndSensor.Device.ModbusTcpDeviceSettingsView(), null);
            navigationService.RegisterView("Settings", "TestBenchSettings", new Settings.Bench.TestBenchSettingsView(), typeof(ViewModels.Settings.Bench.TestBenchSettingsViewModel));
            navigationService.RegisterView("Settings", "UserSettings",new Settings.Users.UsersSettingsView(), typeof(ViewModels.Settings.Users.UsersSettingsViewModel));

            // Reports
            navigationService.RegisterRoot("Reports", "ReportsView", new Reports.ReportsView(), typeof(ViewModels.Reports.ReportsViewModel));
            navigationService.RegisterView("Reports", "SavedReports", new Reports.SavedReportsView(), typeof(ViewModels.Reports.SavedReportsViewModel));
            navigationService.RegisterView("Reports", "ShowSavedReport", new Reports.SavedReportPanelView(), null);

            // Exits
            navigationService.RegisterRoot("Exits", "ExitsView", new ExitView(), typeof(ViewModels.ExitViewModel));
        }
    }
}
