using SOD.Core.Device;
using SOD.Core.Device.Modbus;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Linq;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Device.Dialog
{
    public class EditModbusTcpRegisterDialogViewModel : ReactiveObject
    {
        public EditModbusTcpRegisterDialogViewModel(IDialogService dialogService, ModbusRegister modbusRegister)
        {
            DataType = modbusRegister.DataType;
            Id = modbusRegister.Id;
            Description = modbusRegister.Description;
            Cancel = ReactiveCommand.Create(() =>
            {
                dialogService.CloseAsync(false);
            });
            Save = ReactiveCommand.Create(() =>
            {
                modbusRegister.DataType = DataType;
                modbusRegister.Id = Id;
                modbusRegister.Description = Description;
                dialogService.CloseAsync(true);
            });
        }
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        public ChannelDataType DataType { get; set; }
        [Reactive]
        public string Description { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
