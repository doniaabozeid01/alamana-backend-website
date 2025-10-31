using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Service.CartItem.Dtos;

namespace Alamana.Service.Carts.Dtos
{
    public class GetCartDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string userId { get; set; }
        //public ApplicationUser user { get; set; }
        public DateTime CreateAt { get; set; }

        public List<GetCartItem> cartItems { get; set; }
    }
}
