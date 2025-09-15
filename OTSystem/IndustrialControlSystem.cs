using EasyModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTSystem
{
    internal class IndustrialControlSystem
    {

        private static bool messageReceived = false;
        private static int lastOrderId = -1;

        public void Run()
        {
            Console.WriteLine("Simulated OT system with Modbus support (Robot Arm)");

            Thread modbusThread = new Thread(StartModbusServer);
            modbusThread.IsBackground = true;
            modbusThread.Start();

            while (true)
            {
                if (messageReceived)
                {
                    Console.WriteLine($"Robotarmen plockar order {lastOrderId}...");
                    Thread.Sleep(2000); // Simulera plockning
                    Console.WriteLine($"Order {lastOrderId} har blivit packad!");
                    messageReceived = false;

                }

                Thread.Sleep(1000);
            }
        }
        public static void StartModbusServer() //tar emot order via Modbus
        {
            int port = 502;

            ModbusServer modbusServer = new ModbusServer();
            modbusServer.Port = port; // Set the port number

            modbusServer.HoldingRegistersChanged += (startAddress, numberOfRegisters) =>
            {
                for (int i = 0; i < numberOfRegisters; i++)
                {
                    int orderId = modbusServer.holdingRegisters[startAddress + i];
                    Console.WriteLine($"Order received via Modbus: Id={orderId}");
                    lastOrderId = orderId;
                    messageReceived = true;
                }
            };
            // --- Start the Modbus Server ---
            try
            {
                Console.WriteLine($"Starting EasyModbus TCP Slave on port {port}...");
                modbusServer.Listen();

                Console.WriteLine("EasyModbus TCP Slave started. Press any key to exit.");
                Console.ReadKey(); // Keep the console open until a key is pressed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

            }
        }
    }
}
