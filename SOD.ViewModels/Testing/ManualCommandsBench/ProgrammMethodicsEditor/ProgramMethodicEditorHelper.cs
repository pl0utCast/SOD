using SOD.App.Commands;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands;
using SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor
{
    public static class ProgramMethodicEditorHelper
    {
        public static BaseCommandViewModel CreateCommandViewModel(CommandConfig commandConfig)
        {
            switch (commandConfig.Type)
            {
                case CommandType.FillingBalloon:
                    return new Commands.FillingBalloonCommandViewModel(commandConfig);
                case CommandType.EmptyingBalloon:
                    return new Commands.EmptyingBalloonCommandViewModel(commandConfig);
                case CommandType.FillingCell:
                    return new Commands.FillingCellCommandViewModel(commandConfig);
                case CommandType.EmptyingCell:
                    return new Commands.EmptyingCellCommandViewModel(commandConfig);
                case CommandType.PressureSet:
                    return new Commands.PressureSetCommandViewModel(commandConfig);
                case CommandType.PressureRelease:
                    return new Commands.PressureReleaseCommandViewModel(commandConfig);
                case CommandType.VerticalCell:
                    return new Commands.VerticalCellCommandViewModel(commandConfig);
                case CommandType.HorizontalCell:
                    return new Commands.HorizontalCellCommandViewModel(commandConfig);
                default:
                    break;
            }
            return null;
        }
    }
}
