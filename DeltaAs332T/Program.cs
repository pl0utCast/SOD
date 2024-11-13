using NModbus;
using NModbus.Utility;
using System.Net.Sockets;

namespace DeltaAs332T
{
    class Program
    {
        static IModbusMaster modbusMaster;

        static void Main(string[] args)
        {
            string ipAddress = "192.168.1.5";

            try
            {
                using (var tcpClient = new TcpClient(ipAddress, 502))
                {
                    modbusMaster = new ModbusFactory().CreateMaster(tcpClient);

                    byte slaveID = 1;
                    ushort startAddress = 6096;
                    ushort numOfPoints = 8;

                    //modbusMaster.WriteMultipleRegisters(slaveID, startAddress,
                    //    new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8 });

                    while (true)
                    {
                        // Для чтения с каналов
                        var regs = modbusMaster.ReadHoldingRegisters(slaveID, startAddress, numOfPoints);

                        for (int i = 0; i < regs.Length; i++)
                        {
                            //Console.WriteLine(regs[i]);
                            Console.WriteLine($"{startAddress + i} - {regs[i]}");
                        }

                        Console.WriteLine("----------");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e}");
            }

            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
