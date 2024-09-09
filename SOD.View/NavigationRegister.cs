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
            //navigationService.RegisterView("Testing", "CRSTestParameters", new Testing.CRSBench.TestParametersView(), typeof(ViewModels.Testing.CRSBench.TestParametersViewModel));
            
            //// Valves
            //navigationService.RegisterRoot("Valves", "ValvesView", new Valves.ValvesView(), typeof(ViewModels.Valves.ValvesViewModel));
            //navigationService.RegisterView("Valves", "EditValveView", new Valves.EditValveView(), typeof(ViewModels.Valves.EditValveViewModel));

            // Settings
            //navigationService.RegisterRoot("Settings", "SettingsView", new Settings.SettingsView(), typeof(ViewModels.Settings.SettingsViewModel));
            //navigationService.RegisterView("Settings", "ValveTypeSettingsView", new Settings.Valve.ValveTypeSettingsView(), typeof(ViewModels.Settings.Valve.ValveTypeSettingsViewModel));
            //navigationService.RegisterView("Settings", "IPSettingsView", new Settings.IPSettings.IPSettingsView(), typeof(ViewModels.Settings.IPSettings.IPSettingsViewModel));
            //navigationService.RegisterView("Settings", "EditValveTypeView", new Settings.Valve.EditValveTypeView(), typeof(ViewModels.Settings.Valve.EditValveTypeViewModel));
            //navigationService.RegisterView("Settings", "DeviceAndSensorSettingsView", new Settings.DeviceAndSensor.DeviceSensorSettingsView(), typeof(ViewModels.Settings.DeviceAndSensor.DeviceAndSensorSettingsViewModel));
            //navigationService.RegisterView("Settings", "ModbusTcpDeviceSettings", new Settings.DeviceAndSensor.Device.ModbusTcpDeviceSettingsView(), null);
            //navigationService.RegisterView("Settings", "TestBenchSettings", new Settings.Bench.TestBenchSettingsView(), typeof(ViewModels.Settings.Bench.TestBenchSettingsViewModel));
            //navigationService.RegisterView("Settings", "StandartsSettings", new Settings.Standarts.StandartsSettingsView(), typeof(ViewModels.Settings.Standarts.StandartsSettingsViewModel));
            //navigationService.RegisterView("Settings", "EditStandart", new Settings.Standarts.EditStandartView(), typeof(ViewModels.Settings.Standarts.EditStandartViewModel));
            //navigationService.RegisterView("Settings", "E14140DeviceSettings", new Settings.DeviceAndSensor.Device.E14140DeviceSettingsView(), typeof(ViewModels.Settings.DeviceAndSensor.Device.E14140DeviceSettingsViewModel));
            //navigationService.RegisterView("Settings", "PKTBAImpulseBoardSettings", new Settings.DeviceAndSensor.Device.PKTBAImpulseBoardSettings(), typeof(ViewModels.Settings.DeviceAndSensor.Device.PKTBAImpulseBoardSettingsViewModel));
            //navigationService.RegisterView("Settings", "UserSettings",new Settings.Users.UsersSettingsView(), typeof(ViewModels.Settings.Users.UsersSettingsViewModel));

            // Reports
            //navigationService.RegisterRoot("Reports", "ReportsView", new Reports.ReportsView(), typeof(ViewModels.Reports.ReportsViewModel));
            //navigationService.RegisterView("Reports", "SavedReports", new Reports.SavedReportsView(), typeof(ViewModels.Reports.SavedReportsViewModel));
            //navigationService.RegisterView("Reports", "ShowSavedReport", new Reports.SavedReportPanelView(), null);

            // Exits
            //navigationService.RegisterRoot("Exits", "ExitsView", new ExitView(), typeof(ViewModels.ExitViewModel));
        }
    }
}
