using SOD.App.Mediums;
using SOD.LocalizationService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Commands
{
    public static class CommandsHelper
    {
        public static CommandConfig GetDefault(CommandCollectionType commandCollectionType, CommandType commandType)
        {
            switch (commandCollectionType)
            {
                case CommandCollectionType.Modbus3Post:
                    switch (commandType)
                    {
                        case CommandType.TestMedium:
                            return new CommandConfig() { Type = CommandType.TestMedium };
                        case CommandType.Filling:
                            return new CommandConfig() { Type = CommandType.Filling };
                        case CommandType.PressurizedCavity:
                            return new CommandConfig() { Type = CommandType.PressurizedCavity };
                        case CommandType.LeakControlCavity:
                            return new CommandConfig() { Type = CommandType.LeakControlCavity };
                        case CommandType.SetPressure:
                            return new CommandConfig() { Type = CommandType.SetPressure };
                        case CommandType.Hold:
                            return new CommandConfig() { Type = CommandType.Hold };
                        case CommandType.Registartion:
                            return new CommandConfig() { Type = CommandType.Registartion };
                        case CommandType.PressureRelease:
                            return new CommandConfig() { Type = CommandType.PressureRelease };
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public static string GetCommandName(this CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.TestMedium:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.TestMedium"];
                case CommandType.Filling:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.Filling"];
                case CommandType.PressurizedCavity:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.PressurizedCavity"];
                case CommandType.LeakControlCavity:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.LeakControlCavity"];
                case CommandType.SetPressure:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.SetPressure"];
                case CommandType.Hold:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.Hold"];
                case CommandType.Registartion:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.Registration"];
                case CommandType.PressureRelease:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.PressureRelease"];
                default:
                    return commandType.ToString();
            }
        }

        public static string GetParametersName(this CommandConfig commandConfig)
        {
            var result = string.Empty;
            foreach (var param in commandConfig.Parameters)
            {
                if (param.Value is MediumType mediumType && commandConfig.Type==CommandType.TestMedium)
                {
                    if (mediumType == MediumType.Liquid) result += LocalizationExtension.LocaliztionService["MediumType.Water"];
                    else return result += LocalizationExtension.LocaliztionService["MediumType.Air"];
                }
                if (param.Value is double digs)
                {
                    return result += digs.ToString();
                }
                if (param.Value is string str)
                {
                    return result += str;
                }
                if (param.Value is int integ)
                {
                    return result += integ;
                }
            }
            return result;
        }
    }
}
