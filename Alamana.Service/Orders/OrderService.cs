using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Data.Enums;
using Alamana.Service.Orders.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Service.Orders
{
    public class OrderService : IOrderService
    {
        private readonly AlamanaBbContext _context;

        public OrderService(AlamanaBbContext context)
        {
            _context = context;
        }


        public async Task<int> CreateOrderAsync(OrderRequest request)
        {
            var cart = await _context.Cart
                .Include(c => c.cartItems)
                .ThenInclude(ci => ci.product)
                .FirstOrDefaultAsync(c => c.userId == request.UserId);

            if (cart == null || cart.cartItems == null || !cart.cartItems.Any())
                throw new Exception("سلة التسوق فارغة أو غير موجودة.");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var cartItem in cart.cartItems)
            {
                var itemTotal = cartItem.product.Price * cartItem.Quantity;
                total += itemTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.productId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.product.Price,
                    TotalPrice = itemTotal
                });
            }

            var order = new Order
            {
                userId = request.UserId, // ✅ تخزين UserId
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                CountryId = request.CountryId,
                GovernorateId = request.GovernorateId,
                CityId = request.CityId,
                DistrictId = request.DistrictId,
                Street = request.Street,
                BuildingNumber = request.BuildingNumber,
                Floor = request.Floor,
                Apartment = request.Apartment,
                Landmark = request.Landmark,
                PaymentMethodId = request.PaymentMethodId,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _context.Order.AddAsync(order);

            // 🧹 حذف السلة بعد تأكيد الطلب
            _context.CartItems.RemoveRange(cart.cartItems);
            _context.Cart.Remove(cart);

            await _context.SaveChangesAsync();

            return order.Id;
        }

















        public async Task<List<OrderResponse>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Order
                .Where(o => o.userId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        ImageUrl = oi.Product.Media.FirstOrDefault(x=>x.Type == MediaType.Image).Url ,
                        Price = oi.Product.Price,
                    }).ToList()
                })
                .ToListAsync();
        }


    }

}
