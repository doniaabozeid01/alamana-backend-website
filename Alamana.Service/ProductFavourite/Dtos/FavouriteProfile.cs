using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.ProductFavourite.Dtos
{
    public class FavouriteProfile : Profile
    {
        public FavouriteProfile()
        {
            CreateMap<FavouriteProducts, AddFavouriteDto>().ReverseMap();
            CreateMap<FavouriteProducts, FavouriteDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();

        }
    }
}
