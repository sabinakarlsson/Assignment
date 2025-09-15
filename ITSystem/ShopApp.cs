using ITSystem.Data;
using ITSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem
{
    public class ShopApp
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;

        public ShopApp(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
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
        }


        internal async Task RunMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Huvudmeny:");
                Console.WriteLine("1. Visa alla ordrar");
                Console.WriteLine("2. Skapa ny order");
                Console.WriteLine("3. Visa alla produkter");
                Console.WriteLine("4. Avsluta");
                Console.Write("Välj ett alternativ (1-4): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        var orders = await orderRepository.GetAllOrdersAsync();
                        Console.WriteLine("\n--- Här är alla ordrar ---");
                        foreach (var order in orders)
                        {
                            Console.WriteLine($"OrderId: {order.Id}, KundId: {order.CustomerId}, Företag: {order.CompanyName}, Datum: {order.OrderDate}, Summa: {order.TotalAmount}");
                        }
                        Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
                        Console.ReadKey();
                        break;

                    case "2":
                        //Console.Clear();
                        var products = await productRepository.GetAllProductsAsync();
                        var orderProducts = new List<(Product, int)>();
                        decimal totalAmount = 0;

                        
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("\n--- Skapa ny order ---");

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
                            if (!int.TryParse(qtyInput, out int quantity) || quantity <= 0)
                            {
                                Console.WriteLine("Ogiltig kvantitet");
                                Thread.Sleep(1500);
                                continue;
                            }
                            if (quantity > selectedProduct.Stock)
                            {
                                Console.WriteLine($"Endast {selectedProduct.Stock} st av {selectedProduct.Name} finns i lager.");
                                Thread.Sleep(1500);
                                continue;
                            }

                            orderProducts.Add((selectedProduct, quantity));
                            totalAmount += selectedProduct.Price * quantity;
                            selectedProduct.Stock -= quantity; //uppaterar saldot lokalt

                            //Console.WriteLine($"\n {quantity} st av {selectedProduct.Name} har lagts till i ordern. Totalt hittills: {totalAmount:C}");

                        }

                        if (orderProducts.Count > 0)
                        {
                            var newOrder = new Order
                            {
                                CustomerId = 1, //hårdkodat denna gång
                                CompanyName = "SABKAR",
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
                        break;

                    case "3":
                        var allProducts = await productRepository.GetAllProductsAsync();
                        Console.WriteLine("\n--- Här är alla produkter ---");
                        foreach (var product in allProducts)
                        {
                            Console.WriteLine($"ProduktId: {product.Id}, Namn: {product.Name}, Beskrivning: {product.Description}, Pris: {product.Price}, Aktuellt lagersaldo: {product.Stock}");
                        }
                        Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine("Avslutar programmet.");
                        return;

                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;

                }
            }
        }
    }
}
