using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation
        public ICollection<Order> Orders { get; set; }
    }
}
