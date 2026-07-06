using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.Orders.Dtos
{
    public class OrderItemResponse
    {
        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
