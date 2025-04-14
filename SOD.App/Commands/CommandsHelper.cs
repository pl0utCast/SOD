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
                        case CommandType.FillingBalloon:
                            return new CommandConfig() { Type = CommandType.FillingBalloon };
                        case CommandType.EmptyingBalloon:
                            return new CommandConfig() { Type = CommandType.EmptyingBalloon };
                        case CommandType.FillingCell:
                            return new CommandConfig() { Type = CommandType.FillingCell };
                        case CommandType.EmptyingCell:
                            return new CommandConfig() { Type = CommandType.EmptyingCell };
                        case CommandType.PressureSet:
                            return new CommandConfig() { Type = CommandType.PressureSet };
                        case CommandType.PressureRelease:
                            return new CommandConfig() { Type = CommandType.PressureRelease };
                        case CommandType.VerticalCell:
                            return new CommandConfig() { Type = CommandType.VerticalCell };
                        case CommandType.HorizontalCell:
                            return new CommandConfig() { Type = CommandType.HorizontalCell };
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
                case CommandType.FillingBalloon:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.FillingBalloon"];
                case CommandType.EmptyingBalloon:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.EmptyingBalloon"];
                case CommandType.FillingCell:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.FillingCell"];
                case CommandType.EmptyingCell:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.EmptyingCell"];
                case CommandType.PressureSet:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.PressureSet"];
                case CommandType.PressureRelease:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.PressureRelease"];
                case CommandType.VerticalCell:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.VerticalCell"];
                case CommandType.HorizontalCell:
                    return LocalizationExtension.LocaliztionService["Testing.ManualCommandsBench.HorizontalCell"];
                default:
                    return commandType.ToString();
            }
        }

        public static string GetParametersName(this CommandConfig commandConfig)
        {
            var result = string.Empty;
            foreach (var param in commandConfig.Parameters)
            {
                //if (param.Value is MediumType mediumType && commandConfig.Type==CommandType.TestMedium)
                //{
                //    if (mediumType == MediumType.Liquid) result += LocalizationExtension.LocaliztionService["MediumType.Water"];
                //    else return result += LocalizationExtension.LocaliztionService["MediumType.Air"];
                //}
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
