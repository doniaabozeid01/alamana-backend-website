using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.CartItem.Dtos
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<CartItems,AddCartItem>().ReverseMap();
            CreateMap<CartItems,GetCartItem>().ReverseMap();
        }
    }
}
