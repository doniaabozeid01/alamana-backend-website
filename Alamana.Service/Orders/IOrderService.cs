using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Orders.Dtos;

namespace Alamana.Service.Orders
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderRequest request);
        Task<List<OrderResponse>> GetOrdersByUserIdAsync(string userId);

    }
}
