using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.Advertisment.Dtos
{
    public class AdvertiseProfile : Profile
    {
        public AdvertiseProfile()
        {
            CreateMap<Advertisements, AddAdvertise>().ReverseMap();
            CreateMap<Advertisements, AdvertiseDto>().ReverseMap();
        }
    }
}
