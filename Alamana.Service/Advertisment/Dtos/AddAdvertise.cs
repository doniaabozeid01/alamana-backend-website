using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Advertisment.Dtos
{
    public class AddAdvertise
    {
        public IFormFile? ImageUrl { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        public List<int>? ProductIds { get; set; }
    }
}
