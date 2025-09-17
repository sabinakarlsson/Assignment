using ITSystem.Data;
using ITSystem.Repositories;
using ITSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace ITSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<ShopApp>();
            await app.InitAsync();
            await app.RunMenuAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Warning); // Endast varningar och fel visas
                    })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ShopDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                    services.AddScoped<IOrderRepository, OrderRepository>();
                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IIncidentRepository, IncidentRepository>();
                    services.AddScoped<ShopApp>();
                });
    }
}
