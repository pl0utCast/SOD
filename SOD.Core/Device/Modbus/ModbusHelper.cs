using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.Core.Device.Modbus
{
    static class ModbusHelper
    {
        public static int GetModbusRegisterBytesCount(this ModbusRegister modbusRegister)
        {
            if (modbusRegister.DataType == ChannelDataType.BOOL) return 2;
            if (modbusRegister.DataType == ChannelDataType.INT16) return 2;
            if (modbusRegister.DataType == ChannelDataType.UINT16) return 2;
            if (modbusRegister.DataType == ChannelDataType.INT) return 4;
            if (modbusRegister.DataType == ChannelDataType.UINT) return 4;
            if (modbusRegister.DataType == ChannelDataType.FLOAT) return 4;
            if (modbusRegister.DataType == ChannelDataType.DOUBLE) return 8;
            if (modbusRegister.DataType == ChannelDataType.STRING) return modbusRegister.ByteCount;
            return 0;
        }

        public static object ParseModbusValue(this ModbusRegister modbusRegister, ushort[] data)
        {
            if (modbusRegister.DataType== ChannelDataType.BOOL)
            {
                if (data[0] == 0) return false;
                else return true;
            }
            else if (modbusRegister.DataType == ChannelDataType.INT16)
            {
                return (Int16)data[0];
            }
            else if (modbusRegister.DataType == ChannelDataType.UINT16)
            {
                return data[0];
            }
            else if (modbusRegister.DataType == ChannelDataType.INT)
            {
                return (int)NModbus.Utility.ModbusUtility.GetUInt32(data[1], data[0]);
            }
            else if (modbusRegister.DataType == ChannelDataType.UINT)
            {
                return (Int32)NModbus.Utility.ModbusUtility.GetUInt32(data[1], data[0]);
            }
            else if (modbusRegister.DataType == ChannelDataType.FLOAT)
            {
                return BitConverter.Int32BitsToSingle((int)NModbus.Utility.ModbusUtility.GetUInt32(data[1], data[0]));
            }
            else if (modbusRegister.DataType == ChannelDataType.DOUBLE)
            {
                return NModbus.Utility.ModbusUtility.GetDouble(data[3], data[2], data[1], data[0]);
            }
            else if (modbusRegister.DataType == ChannelDataType.STRING)
            {
                var asciiBytes = NModbus.Utility.ModbusUtility.GetAsciiBytes(data);
                return ASCIIEncoding.ASCII.GetString(asciiBytes);
            }
            return 0;
        }
    }
}
