using BCrypt.Net;
using ITSystem.Data;
using ITSystem.Repositories;
using ITSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITSystem
{
    public class ShopApp
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;
        private readonly IIncidentRepository incidentRepository;

        private User? loggedInUser;

        public ShopApp(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, IIncidentRepository incidentRepository)
        {
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.userRepository = userRepository;
            this.incidentRepository = incidentRepository;
        }

        internal async Task InitAsync() //säkerställa att databasen är skapad, om är skapad (men inga produkter) så skapa dessa produkter
        {
            await productRepository.EnsureDatabaseCreatedAsync();

            if (await productRepository.CountAsync() == 0)
            {
                var products = new List<Product>
                {
                    new Product { Name = "Mobilskal", Description = "Passar Samsung s24", Price = 349, Stock = 1500 },
                    new Product { Name = "Laddkabel", Description = "USB-C - USB C", Price = 149, Stock = 1500},
                    new Product { Name = "Skärmskydd", Description = "Passar Samsung s24", Price = 179, Stock = 1500 },
                    new Product { Name = "Trådlös Laddare", Description = "Snabbladdning", Price = 299, Stock = 1500 },
                    new Product { Name = "Powerbank", Description = "2xUSB-A, USB-C", Price = 499, Stock = 1500 },
                    new Product { Name = "Bluetooth Hörlurar", Description = "20h speltid", Price = 899, Stock = 1500 },
                    new Product { Name = "Smartwatch", Description = "Vattentät, pulsmätare, GPS", Price = 1999, Stock = 1500 },
                    new Product { Name = "Surfplatta", Description = "10.5 tum, 64GB, WiFi", Price = 2999, Stock = 1500 },
                    new Product { Name = "Bärbar Högtalare", Description = "Vattentät, 12h speltid", Price = 699, Stock = 1500 },
                    new Product { Name = "USB-minne", Description = "128GB, USB 3.0", Price = 249, Stock = 1500 }
                };

                await productRepository.AddRangeAsync(products);
            }

            var users = await userRepository.GetAllUsersAsync();
            if (!users.Any())
            {
                Console.WriteLine("Skapa adminanvändare!");
                string username;
                while (true)
                {
                    Console.Write("Användarnamn ");
                    username = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(username)) break;
                    Console.WriteLine("Användarnamn får inte vara tomt");
                }

                string password;
                while (true)
                {
                    Console.Write("Lösenord: ");
                    password = ReadPassword();
                    if (ValidatePassword(password, out var error)) break;
                    
                    Console.WriteLine(error);
                }

                var admin = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    IsAdmin = true
                };

                await userRepository.AddUserAsync(admin);
                Console.WriteLine("Adminanvändare skapad. Tryck på valfri tangent för att fortsätta.");
                Console.ReadKey();

            }
        }


        internal async Task RunMenuAsync()
        {

            await LoginAsyncLoop();


            while (true)
            {
                Console.Clear();
                Console.WriteLine("Huvudmeny:");
                Console.WriteLine("1. Visa alla ordrar");
                Console.WriteLine("2. Skapa ny order");
                Console.WriteLine("3. Visa alla produkter");
                Console.WriteLine("4. Visa incidentloggar");
                Console.WriteLine("5. Avsluta");
                Console.Write("Välj ett alternativ (1-5): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowOrdersAsync();
                        break;

                    case "2":
                        await CreateOrderAsync();
                        break;

                    case "3":
                        await ShowProductsAsync();
                        break;

                    case "4":
                        await ShowLoggedIncidentsAsync();
                        break;

                    case "5":
                        Console.WriteLine("Avslutar programmet.");
                        return;

                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;

                }
            }
        }


        internal async Task<User> LoginAsyncLoop()
        {
            while (loggedInUser == null)
            {
                loggedInUser = await LogInAsync();
            }

            Console.WriteLine($"Välkommen, {loggedInUser.Username}!");
            Thread.Sleep(1000);

            return loggedInUser;
        }

        private async Task<User?> LogInAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Inloggning ---");

            Console.Write("Ange användarnamn: ");
            var username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            var password = ReadPassword();

            var user = await userRepository.GetUserByUsernameAsync(username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {

                await incidentRepository.AddIncidentAsync(new Incident
                {
                    Type = "LoginPailure",
                    Message = "Misslyckat inloggningsförsök",
                    Username = username
                });

                Console.WriteLine("Ogiltigt användarnamn eller lösenord.");
                return null;
            }

            return user;
        }

        private string ReadPassword()
        {
            var password = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }

            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }

        private bool ValidatePassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{10,}$";

            if (!Regex.IsMatch(password, pattern))
            {
                errorMessage = "Lösenordet måste vara minst 10 tecken långt och innehålla minst en stor bokstav, en liten bokstav och ett specialtecken.";
                return false;
            }

            return true;

        }

        private async Task ShowOrdersAsync()
        {
            Console.Clear();
            var orders = await orderRepository.GetAllOrdersAsync();
            Console.WriteLine("\n--- Här är alla ordrar ---");
            foreach (var order in orders)
            {
                Console.WriteLine($"OrderId: {order.Id}, KundId: {order.CompanyId}, Företag: {order.CompanyName}, Datum: {order.OrderDate}, Summa: {order.TotalAmount}");
            }
            Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
            Console.ReadKey();
        }

        private async Task CreateOrderAsync()
        {
            var products = await productRepository.GetAllProductsAsync();
            var orderProducts = new List<(Product, int)>();
            decimal totalAmount = 0;

            Console.Clear();
            Console.WriteLine("\n--- Skapa ny order ---");

            Console.Write("Id på företaget som beställer (ex SABKAR för SabinaKarlsson): ");
            var companyId = Console.ReadLine();

            Console.Write("Företagets namn: ");
            var companyName = Console.ReadLine();

            while (true)
            {

                Console.WriteLine("\nProdukter i databasen: ");
                foreach (var product in products)
                {
                    Console.WriteLine($"ProduktId: {product.Id}, Namn: {product.Name}, Pris: {product.Price}, Aktuellt lagersaldo: {product.Stock}");
                }

                if (orderProducts.Any())
                {
                    foreach (var (product, qty) in orderProducts)
                    {
                        Console.WriteLine($"Tillagt i ordern: {qty} st av {product.Name}");
                    }
                    Console.WriteLine($"Totalt hittills: {totalAmount:C}\n");
                }

                Console.Write("Ange Id för den produkt du vill ha (alternativt 'klar' för att avsluta) : ");
                var input = Console.ReadLine();
                if (input?.ToLower() == "klar") break;

                if (!int.TryParse(input, out int productId)) continue;

                var selectedProduct = products.FirstOrDefault(p => p.Id == productId);
                if (selectedProduct == null)
                {
                    Console.WriteLine("Ogiltigt produktId");
                    Thread.Sleep(1500);
                    continue;
                }

                Console.WriteLine($"Hur många vill du beställa av {selectedProduct.Name}? ");
                var qtyInput = Console.ReadLine();
                if (!int.TryParse(qtyInput, out int quantity))
                {
                    Console.WriteLine("Ogiltig kvantitet");
                    Thread.Sleep(1500);
                    continue;
                }
                if (!await ValidateOrderQuantityAsync(selectedProduct, quantity, loggedInUser))
                {
                    Thread.Sleep(1500);
                    continue;
                }

                orderProducts.Add((selectedProduct, quantity));
                totalAmount += selectedProduct.Price * quantity;
                selectedProduct.Stock -= quantity; //uppaterar saldot lokalt


            }

            if (orderProducts.Count > 0)
            {
                var newOrder = new Order
                {
                    CompanyId = companyId,
                    CompanyName = companyName,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount
                };

                await orderRepository.AddOrderAsync(newOrder);

                foreach (var (product, _) in orderProducts)
                {
                    await productRepository.UpdateProductAsync(product); //uppdaterar lagersaldo i databasen
                }

                Console.WriteLine("Ordern skapad!");
            }

            else
            {
                Console.WriteLine("Ingen order skapades.");
            }

            Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
            Console.ReadKey();
        }

        private async Task<bool> ValidateOrderQuantityAsync(Product products, int quantity, User? loggedInUser)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Kvantiteten måste vara större än noll.");
                return false;
            }

            if (quantity > 1000)
            {
                await incidentRepository.AddIncidentAsync(new Incident
                {
                    Type = "HighQuantityOrder",
                    Message = $"Användare {loggedInUser?.Username} försökte beställa {quantity} st av {products.Name}",
                    Username = loggedInUser?.Username ?? "Okänd",
                    IncidentTimestamp = DateTime.Now
                });

                Console.WriteLine("Du kan inte beställa mer än 1000 enheter av en produkt.");
                return false;
            }
            if (quantity > products.Stock)
            {
                Console.WriteLine($"Endast {products.Stock} st av {products.Name} finns i lager.");
                return false;
            }

            return true;
        }

        private async Task ShowLoggedIncidentsAsync()
        {
            if (!loggedInUser.IsAdmin)
            {
                Console.WriteLine("Endast admin kan se incidentloggar.");
                Console.ReadKey();
                return;
            }

            var incidents = await incidentRepository.GetAllIncidentsAsync();
            Console.WriteLine("\n--- Incidentlogg ---");
            foreach (var incident in incidents)
            {
                Console.WriteLine($"Id: {incident.Id}, Typ: {incident.Type}, Användare: {incident.Username}, Tid: {incident.IncidentTimestamp}, Meddelande: {incident.Message}");
            }
            Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
            Console.ReadKey();
        }

        private async Task ShowProductsAsync()
        {
            var allProducts = await productRepository.GetAllProductsAsync();
            Console.WriteLine("\n--- Här är alla produkter ---");
            foreach (var product in allProducts)
            {
                Console.WriteLine($"ProduktId: {product.Id}, Namn: {product.Name}, Beskrivning: {product.Description}, Pris: {product.Price}, Aktuellt lagersaldo: {product.Stock}");
            }
            Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
            Console.ReadKey();
        }
    }
}
