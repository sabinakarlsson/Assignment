using ITSystem.Data; //koppling till DbCOntext och Order
using EasyModbus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Bygg konfigurering för att läsa appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Setup DI container
            var services = new ServiceCollection();

            // Lägger till DbContext
            services.AddDbContext<ShopDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Lägger till ModbusClient
            services.AddSingleton(new ModbusClient("127.0.0.1", 502));

            // Lägger till IntegrationService
            services.AddTransient<IntegrationService>();

            var serviceProvider = services.BuildServiceProvider();

            // Kör integrationen
            var integrationService = serviceProvider.GetRequiredService<IntegrationService>();
            integrationService.SendOrdersToOT();
        }
    }

    public class IntegrationService //DI av ModbusClient och DbContext + kan loopa i databasen (inte enbart orderId) + sparar i databasen att ordern är skickad
    {
        private readonly ShopDbContext _db;
        private readonly ModbusClient _modbusClient;

        public IntegrationService(ShopDbContext db, ModbusClient modbusClient)
        {
            _db = db;
            _modbusClient = modbusClient;
            _modbusClient.Connect(); // Ansluter till Modbus-servern när tjänsten skapas
        }

        public void SendOrdersToOT()
        {
            Console.WriteLine("IntegrationSystem startat. Väntar på nya order...");

            while (true)
            {
                var orders = _db.Orders.Where(o => !o.SentToOT).ToList();

                foreach (var order in orders)
                {
                    try
                    {
                        if (!_modbusClient.Connected)
                        {
                            _modbusClient.Connect();

                            _modbusClient.WriteSingleRegister(0, order.Id);
                            Console.WriteLine($"Order {order.Id} skickad till OT-systemet!");

                            bool confirmed = false;

                            while (!confirmed)
                            {
                                Thread.Sleep(1000); // Väntar 1 sekund innan nästa kontroll
                                int[] response = _modbusClient.ReadHoldingRegisters(1, 1);
                                int confirmedOrderId = response[0];

                                if (confirmedOrderId == order.Id)
                                {
                                    Console.WriteLine($"Order {order.Id} bekräftad av OT-systemet.");
                                    order.SentToOT = true;
                                    confirmed = true;
                                }
                                else
                                {
                                    Console.WriteLine($"Väntar på bekräftelse för order {order.Id}...");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Kunde inte ansluta till Modbus-servern: {ex.Message}");
                        continue; // Hoppa över denna order och försök igen senare
                    }

                }

                _db.SaveChanges();

                Thread.Sleep(2000); // Väntar 2 sekunder innan nästa kontroll
            }

        }
    }
}
