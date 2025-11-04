using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Products : BaseEntity  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public bool New { get; set; } = false;
        public ICollection<ProductMedia> Media { get; set; } = new List<ProductMedia>();
        public int CategoryId { get; set; }
        public Categories Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
