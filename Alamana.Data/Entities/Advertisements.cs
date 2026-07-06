using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Advertisements : BaseEntity
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        public ICollection<AdvertisementProduct> AdvertisementProducts { get; set; } = new List<AdvertisementProduct>();
        public ICollection<CountryAdvertisements> CountryAdvertisements { get; set; } = new List<CountryAdvertisements>();
    }
}
