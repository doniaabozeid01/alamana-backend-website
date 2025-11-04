using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Data.Enums;
using AutoMapper;

namespace Alamana.Service.Product.Dtos
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Products, ProductDto>().ReverseMap();
            CreateMap<Products, AddProductDto>().ReverseMap();

            CreateMap<Products, ProductDto>()
            .ForMember(d => d.GalleryUrls, opt => opt.MapFrom(s =>
                s.Media
                 .Where(m => m.Type == MediaType.Image)
                 .Select(m => m.Url)
                 .ToList()
            ))
            .ReverseMap();

            CreateMap<Products, AddProductDto>().ReverseMap();
        }
    }
}
