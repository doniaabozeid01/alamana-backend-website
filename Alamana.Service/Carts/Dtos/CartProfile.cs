using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.Carts.Dtos
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Alamana.Data.Entities.Cart, AddCartDto>().ReverseMap();
            CreateMap<Alamana.Data.Entities.Cart, GetCartDto>().ReverseMap();
        }
    }
}
