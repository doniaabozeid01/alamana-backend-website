using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Service.CartItem.Dtos
{
    public class GetCartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ImagePath { get; set; }
        public int productId { get; set; }
        //public Products product { get; set; }
        public int cartId { get; set; }
        //public Cart Cart { get; set; }
    }
}
