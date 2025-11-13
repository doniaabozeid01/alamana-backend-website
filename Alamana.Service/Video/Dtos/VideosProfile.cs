using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.Video.Dtos
{
    public class VideosProfile : Profile
    {
        public VideosProfile()
        {
            CreateMap<Videos, VideoDto>();
            CreateMap<CreateVideoDto, Videos>();
            CreateMap<UpdateVideoDto, Videos>();
        }
    }
}
