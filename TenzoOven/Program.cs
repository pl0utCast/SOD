using NModbus;
using NModbus.Serial;
using NModbus.Utility;
using System.IO.Ports;

namespace TenzoOven
{
    class Program
    {
        private static SerialPort serialPort;
        private static IModbusMaster modbusMaster;

        static void Main(string[] args)
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 9600; //9600
            serialPort.Parity = Parity.Even;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.ReadTimeout = 100;
            try
            {
                serialPort.PortName = "COM6";
                serialPort.Open();
                if (!serialPort.IsOpen) return;
                modbusMaster = new ModbusFactory().CreateRtuMaster(serialPort);

                while (true)
                {
                    try
                    {
                        // 0x3E - 0x3F – 1 канал
                        // 0x40 - 0x41 – 2 канал
                        // 0x42 - 0x43 – 3 канал
                        // 0x44 - 0x45 – 4 канал
                        var registers1 = modbusMaster.ReadHoldingRegisters(1, 0x40, 2);
                        var impulse = ModbusUtility.GetSingle(registers1[0], registers1[1]);
                        Console.WriteLine(impulse);

                        //var registers = modbusMaster.ReadHoldingRegisters(1, 0x3E, 2);
                        //foreach (var reg in registers)
                        //    Console.WriteLine(reg);
                    }
                    catch (InvalidOperationException te)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (serialPort.IsOpen) serialPort.Close();
            }

            //Console.ReadLine();
            //Environment.Exit(0);
        }
    }
}
