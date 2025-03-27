using SOD.App.Commands;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands;
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
                case CommandType.TestMedium:
                    return new TestMediumCommandViewModel(commandConfig);
                case CommandType.Filling:
                    return new FillingCommandViewModel(commandConfig);
                case CommandType.PressurizedCavity:
                    return new PressurizeCavityCommandViewModel(commandConfig);
                case CommandType.LeakControlCavity:
                    return new LeakControlCavityCommandViewModel(commandConfig);
                case CommandType.SetPressure:
                    return new SetPressureCommandViewModel(commandConfig);
                case CommandType.Hold:
                    return new HoldCommandViewModel(commandConfig);
                case CommandType.Registartion:
                    return new RegistrationCommandViewModel(commandConfig);
                case CommandType.PressureRelease:
                    return new PressureRealeseCommandViewModel(commandConfig);
                case CommandType.Purge:
                    return new PurgeCommandViewModel(commandConfig);
                default:
                    break;
            }
            return null;
        }
    }
}
