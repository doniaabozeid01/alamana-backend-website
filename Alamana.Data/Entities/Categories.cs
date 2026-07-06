using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Categories : BaseEntity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string ImagePath { get; set; }
        public string? MobileImagePath { get; set; }
        public List<Products> Products { get; set; }
        public ICollection<CountryCategories> CountryCategories { get; set; } = new List<CountryCategories>();
    }
}
