using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOD.Core.Device.Modbus
{
    public static class ModbusDeviceExtensions
    {
        public static async Task CreateTriggerAsync(this ModbusTcpDevice modbusTcpDevice, ushort regId, Func<ushort[], bool> condition, Action<ushort[]> afterAction, CancellationToken cancellationToken)
        {
            var waiting = true;
            while (waiting)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var regData = await modbusTcpDevice.ReadHoldingRegistersAsync(regId, 1);
                if (regData == null) return;

                waiting = !condition(regData);
                if (waiting == false)
                {
                    afterAction(regData);
                }
                await Task.Delay(500, cancellationToken);
            }
        }

        public static async Task CreateFloatTriggerAsync(this ModbusTcpDevice modbusTcpDevice, ushort regId, Func<float, bool> condition, Action<float> afterAction, CancellationToken cancellationToken)
        {
            var waiting = true;
            while (waiting)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var regData = await modbusTcpDevice.ReadHoldingRegistersAsync(regId, 2);
                if (regData == null) return;

                float val = BitConverter.ToSingle(BitConverter.GetBytes(regData[0]).Concat(BitConverter.GetBytes(regData[1])).ToArray(), 0);

                waiting = !condition(val);
                if (waiting == false)
                {
                    afterAction(val);
                }
                await Task.Delay(500, cancellationToken);
            }
        }

        public static async Task WriteInt32(this ModbusTcpDevice modbusTcpDevice, ushort regId, int data)
        {
            var bytes = BitConverter.GetBytes(data);
            var sendArray = new ushort[]
            {
                        BitConverter.ToUInt16(bytes, 0),
                        BitConverter.ToUInt16(bytes, 2)
            };
            await modbusTcpDevice.WriteHoldingRegistersAsync(regId, sendArray);
        }

        public static async Task WriteSingle(this ModbusTcpDevice modbusTcpDevice, ushort regId, float data)
        {
            var bytes = BitConverter.GetBytes(data);
            var sendArray = new ushort[]
            {
                        BitConverter.ToUInt16(bytes, 0),
                        BitConverter.ToUInt16(bytes, 2)
            };
            await modbusTcpDevice.WriteHoldingRegistersAsync(regId, sendArray);
        }

        public static async Task WriteDouble(this ModbusTcpDevice modbusTcpDevice, ushort regId, double data)
        {
            var bytes = BitConverter.GetBytes(data);
            var sendArray = new ushort[]
            {
                        BitConverter.ToUInt16(bytes, 0),
                        BitConverter.ToUInt16(bytes, 2),
                        BitConverter.ToUInt16(bytes, 4),
                        BitConverter.ToUInt16(bytes, 6)
            };
            await modbusTcpDevice.WriteHoldingRegistersAsync(regId, sendArray);
        }
    }
}
