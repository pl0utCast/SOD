using SOD.Dialogs;

namespace SOD.View
{
    public static class DialogRegister
    {
        public static void Register(IDialogService dialogService)
        {
            dialogService.RegisterDialog("EditBalloonProperty", new Settings.Balloon.Dialog.EditBalloonPropertyDialog());

            //dialogService.RegisterDialog("AddListStringItem", new Valves.Dialog.AddListStringItemView());

            dialogService.RegisterDialog("EditModbusTcpRegister", new Settings.DeviceAndSensor.Device.Dialog.EditModbusTcpRegisterView());
            dialogService.RegisterDialog("EditOvenMBDeviceRegister", new Settings.DeviceAndSensor.Device.Dialog.EditOvenMBDeviceRegisterView());
            //dialogService.RegisterDialog("ValueBasedSensorSettings", new Settings.DeviceAndSensor.Sensors.Dialog.ValueBasedSensorSettingsView());
            dialogService.RegisterDialog("EditProperty", new Props.EditPropertyView());
            dialogService.RegisterDialog("DeleteProperty", new Props.DeletePropView());
            //dialogService.RegisterDialog("SaveDeviceSettings", new Settings.DeviceAndSensor.Device.Dialog.SaveDeviceSettingsView());
            dialogService.RegisterDialog("CodeBasedSensorSettings", new Settings.DeviceAndSensor.Sensors.Dialog.CodeBasedSensorSettingsView());
            dialogService.RegisterDialog("TenzoSensorSettings", new Settings.DeviceAndSensor.Sensors.Dialog.TenzoSensorSettingsView());
            dialogService.RegisterDialog("CoefficentCalibration", new Settings.DeviceAndSensor.Sensors.Dialog.CoefficentCalibrationView());
            dialogService.RegisterDialog("AddLangView", new Settings.Bench.Dialogs.AddLangView());
            dialogService.RegisterDialog("EditUser", new Settings.Users.Dialogs.EditUserView());
            dialogService.RegisterDialog("DeleteUser", new Settings.Users.Dialogs.DeleteUserView());

            dialogService.RegisterDialog("DeleteProgrammMethodics", new Testing.ManualCommandsBench.Dialog.DeleteProgrammMethodicsView());
            dialogService.RegisterDialog("AddCommand", new Testing.ManualCommandsBench.ProgrammMethodicsEditor.Dialogs.AddCommandDialog());

            dialogService.RegisterDialog("DeleteReportDialog", new Reports.Dialogs.DeleteReportsDialogView());
            dialogService.RegisterDialog("ExportReportDialog", new Reports.Dialogs.ExportReportDialogView());

            dialogService.RegisterDialog("SODBenchTestResult", new Testing.SODBench.Dialogs.TestResultView());
            dialogService.RegisterDialog("CreateNewReport", new Testing.SODBench.Dialogs.CreateNewReport());

            dialogService.RegisterDialog("IncorrectPassword", new Dialog.IncorrectPasswordView());
        }
    }
}
