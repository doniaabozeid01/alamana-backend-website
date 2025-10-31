using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Cart : BaseEntity
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string userId { get; set; }
        public ApplicationUser user { get; set; }
        public DateTime CreateAt { get; set; }
        public List<CartItems> cartItems { get; set; }
    }
}
