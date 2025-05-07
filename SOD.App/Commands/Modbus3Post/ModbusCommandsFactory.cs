using MemBus;
using SOD.Core.Balloons;
using SOD.Core.Device.Modbus;
using SOD.LocalizationService;

namespace SOD.App.Commands.Modbus3Post
{
    public class ModbusCommandsFactory : CommandsFactory
    {
        private ModbusTcpDevice _modbusTcpDevice;
        private readonly ILocalizationService _localizationService;
        private readonly IBus _bus;
        public ModbusCommandsFactory(ModbusTcpDevice modbusTcpDevice, IBus bus, ILocalizationService localizationService)
        {
            _modbusTcpDevice = modbusTcpDevice;
            _localizationService = localizationService;
            _bus = bus;
        }
        public override ICommand Create(CommandConfig commandConfig, Balloon valve)
        {
            var parameters = commandConfig.Parameters.Select(kv => kv.Value).ToArray();
            switch (commandConfig.Type)
            {
                case CommandType.FillingBalloon:
                    return new FillingBalloonCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.EmptyingBalloon:
                    return new EmptyingBalloonCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.FillingCell:
                    return new FillingCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.EmptyingCell:
                    return new EmptyingCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.PressureSet:
                    return new PressureSetCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.PressureRelease:
                    return new PressureReleaseCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.VerticalCell:
                    return new VerticalCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                case CommandType.HorizontalCell:
                    return new HorizontalCellCommand(_modbusTcpDevice, _bus, commandConfig, _localizationService, parameters);
                default:
                    break;
            }
            return null;
        }
    }
}
