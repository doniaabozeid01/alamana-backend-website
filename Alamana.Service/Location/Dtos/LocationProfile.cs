using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Service.Category.Dtos;
using AutoMapper;

namespace Alamana.Service.Location.Dtos
{
    public class LocationProfile : Profile
    {
        public LocationProfile() 
        {
            CreateMap<country, GetCountryDto>().ReverseMap();
            CreateMap<Governorate, GetGovernorateDto>().ReverseMap();
            CreateMap<city, GetCityDto>().ReverseMap();
            CreateMap<District, GetDestrictDto>().ReverseMap();
        }

    }
}
