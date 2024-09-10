using SOD.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.View
{
    public static class DialogRegister
    {
        public static void Register(IDialogService dialogService)
        {
            //dialogService.RegisterDialog("EditValveProperty", new Settings.Valve.Dialog.EditVavlePropertyDialog());
            //dialogService.RegisterDialog("DeleteValveProperty", new Settings.Valve.Dialog.DeleteValvePropertyDialog());
            //dialogService.RegisterDialog("DeleteValveType", new Settings.Valve.Dialog.DeleteValveTypeDialog());

            //dialogService.RegisterDialog("AddListStringItem", new Valves.Dialog.AddListStringItemView());
            //dialogService.RegisterDialog("DeleteValve", new Valves.Dialog.DeleteValveDialog());

            //dialogService.RegisterDialog("EditModbusTcpRegister", new Settings.DeviceAndSensor.Device.Dialog.EditModbusTcpRegisterView());
            //dialogService.RegisterDialog("ValueBasedSensorSettings", new Settings.DeviceAndSensor.Sensors.Dialog.ValueBasedSensorSettingsView());
            //dialogService.RegisterDialog("ScriptSelector", new Settings.Standarts.Dialog.ScriptSelectorView());
            //dialogService.RegisterDialog("SelectPostSensor", new Settings.Bench.ThreePostBench.Dialog.SelectPostSensorView());
            //dialogService.RegisterDialog("AddTestSettings", new Settings.Bench.ThreePostBench.Dialog.AddTestView());
            //dialogService.RegisterDialog("EditProperty", new Props.EditPropertyView());
            //dialogService.RegisterDialog("DeleteProperty", new Props.DeletePropView());
            //dialogService.RegisterDialog("SaveDeviceSettings", new Settings.DeviceAndSensor.Device.Dialog.SaveDeviceSettingsView());
            //dialogService.RegisterDialog("CodeBasedSensorSettings", new Settings.DeviceAndSensor.Sensors.Dialog.CodeBasedSensorSettingsView());
            //dialogService.RegisterDialog("AddLangView", new Settings.Bench.Dialogs.AddLangView());
            dialogService.RegisterDialog("EditUser", new Settings.Users.Dialogs.EditUserView());
            dialogService.RegisterDialog("DeleteUser", new Settings.Users.Dialogs.DeleteUserView());

            dialogService.RegisterDialog("DeleteReportDialog", new Reports.Dialogs.DeleteReportsDialogView());
            dialogService.RegisterDialog("ExportReportDialog", new Reports.Dialogs.ExportReportDialogView());

            //dialogService.RegisterDialog("CRSBenchTestResult", new Testing.CRSBench.Dialogs.TestResultView());
            //dialogService.RegisterDialog("CreateNewReport", new Testing.CRSBench.Dialogs.CreateNewReport());
            //dialogService.RegisterDialog("CheckSumError", new Testing.CRSBench.Dialogs.CheckSumError());

            dialogService.RegisterDialog("IncorrectPassword", new Dialog.IncorrectPasswordView());
        }
    }
}
