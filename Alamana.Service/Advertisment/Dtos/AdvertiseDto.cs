using System;
using System.Collections.Generic;

namespace Alamana.Service.Advertisment.Dtos
{
    public class AdvertiseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        public IReadOnlyList<int> ProductIds { get; set; } = Array.Empty<int>();
    }
}
