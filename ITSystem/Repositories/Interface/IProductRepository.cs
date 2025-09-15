using ITSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Repositories.Interface
{
    public interface IProductRepository
    {
        Task EnsureDatabaseCreatedAsync();
        Task<int> CountAsync();
        Task<List<Product>> GetAllProductsAsync();
        Task AddRangeAsync(IEnumerable<Product> products);
        Task UpdateProductAsync(Product product);
    }
}
