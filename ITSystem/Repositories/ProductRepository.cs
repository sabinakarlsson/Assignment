using ITSystem.Data;
using ITSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopDbContext _context;
        public ProductRepository(ShopDbContext context)
        {
            _context = context;
        }
        

        public async Task EnsureDatabaseCreatedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
