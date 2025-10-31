using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.CartItem.Dtos;

namespace Alamana.Service.Carts.Dtos
{
    public class AddCartItemResultDto
    {
        public GetCartItem Item { get; set; }
        public CartSummaryDto CartSummary { get; set; }
    }
}
