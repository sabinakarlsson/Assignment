using ITSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Repositories.Interface //definierar VAD en OrderRepository ska kunna göra (inte hur)
                                          // som ett kontrakt som en klass som implementerar interfacet måste följa
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task AddOrderAsync(Order order);
    }
}
