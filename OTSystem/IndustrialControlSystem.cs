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
        private static ModbusServer modbusServer;

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

            modbusServer = new ModbusServer();
            modbusServer.Port = port; // sätter portnumret


            modbusServer.HoldingRegistersChanged += (startAddress, numberOfRegisters) =>
            {
                //HÄR123 Console.WriteLine($"HoldingRegistersChanged fired! startAddress={startAddress}, numberOfRegisters={numberOfRegisters}"); //HÄR123
                int orderId = modbusServer.holdingRegisters[startAddress];
                if (orderId == 0)
                    return;

                Console.WriteLine($"Order received via Modbus: Id={orderId}");
                lastOrderId = orderId;
                messageReceived = true;


                modbusServer.holdingRegisters[startAddress] = (short)orderId;
                Console.WriteLine($"Bekräftelse skriven till register {startAddress}: {modbusServer.holdingRegisters[startAddress]}");

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
