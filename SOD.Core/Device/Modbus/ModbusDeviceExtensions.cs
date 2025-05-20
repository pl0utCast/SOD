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

        /// <summary>
        /// <para>D30.0 - Подтверждение шага</para>
        /// <para>D30.1 - Пауза шага</para>
        /// <para>D30.2 - Закончить испытание</para>
        /// <para>D30.3 - Режим отладки</para>
        /// <para>D30.4 - Автоматический режим</para>
        /// <para>D30.5 - Отключение зуммера</para>
        /// <para>D30.6 - Запуск на исполнение команды</para>
        /// <para>D30.7 - Тест связи с верхним уровнем</para>
        /// <para>D30.8 - Настройка аварийной остановки</para>
        /// <para>D30.9 - Открытие 5кг колбы в ручном режиме</para>
        /// <para>D30.10 - Открытие 10кг колбы в ручном режиме</para>
        /// <para>D30.11 - Открытие 30кг колбы в ручном режиме</para>
        /// <para>D30.12 - Активация диагностики резкого падения давления</para>
        /// <para>D30.13 - Установить камеру вертикально в ручном режиме</para>
        /// <para>D30.14 - Установить камеру горизонтально в ручном режиме</para>
        /// <para>D30.15 - 0-полное объемное расширение, 1-остаточное</para>
        /// 
        /// <para>D31.0 - Заполнить баллон</para>
        /// <para>D31.1 - Опорожнить баллон</para>
        /// <para>D31.2 - Заполнить камеру</para>
        /// <para>D31.3 - Опорожнить камеру</para>
        /// <para>D31.4 - Набор давления в ручном режиме</para>
        /// <para>D31.5 - Сброс давления в ручном режиме</para>
        /// <para>D31.6 - Запомнить вес тары 5кг</para>
        /// <para>D31.7 - Запомнить вес тары 10кг</para>
        /// <para>D31.8 - Запомнить вес тары 30кг</para>
        /// </summary>
        public static async Task SetOneMaskWord(this ModbusTcpDevice modbusTcpDevice, ushort reg, ushort mask)
        {
            ushort[] regData = await modbusTcpDevice.ReadHoldingRegistersAsync(reg, 1);

            if (regData == null) return;

            ushort val = regData[0];
            val |= mask; // Делаем OR(выставляем в единицы)

            await modbusTcpDevice.WriteHoldingRegistersAsync(reg, [val]);
        }

        /// <summary>
        /// Выставляет нули
        /// </summary>
        public static async Task SetZeroMaskWord(this ModbusTcpDevice modbusTcpDevice, int reg, int mask)
        {
            ushort[] regData = await modbusTcpDevice.ReadHoldingRegistersAsync((ushort)reg, 1);

            if (regData == null) return;

            int val = regData[0];
            val &= mask; // Делаем AND(выставляем в нули)

            await modbusTcpDevice.WriteHoldingRegistersAsync((ushort)reg, [(ushort)val]);
        }
    }
}
