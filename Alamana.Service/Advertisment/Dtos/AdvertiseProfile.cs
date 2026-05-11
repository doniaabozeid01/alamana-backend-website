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
            CreateMap<Advertisements, AdvertiseDto>()
                .ForMember(d => d.ProductIds, o => o.MapFrom(s =>
                    s.AdvertisementProducts
                        .OrderBy(ap => ap.Id)
                        .Select(ap => ap.ProductId)
                        .ToList()));
        }
    }
}
