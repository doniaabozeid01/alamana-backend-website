using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Products Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }  // السعر وقت الطلب
        public decimal TotalPrice { get; set; } // Quantity * UnitPrice
    }
}
