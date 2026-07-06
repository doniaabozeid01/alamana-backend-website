using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Products : BaseEntity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Weight { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public ICollection<ProductMedia> Media { get; set; } = new List<ProductMedia>();
        public ICollection<ProductDetailEntry> DetailEntries { get; set; } = new List<ProductDetailEntry>();
        public int CategoryId { get; set; }
        public Categories Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<AdvertisementProduct> AdvertisementProducts { get; set; } = new List<AdvertisementProduct>();
        public ICollection<CountryProducts> CountryProducts { get; set; } = new List<CountryProducts>();
    }
}

