using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Category.Dtos;

namespace Alamana.Service.Product.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool New { get; set; }
        public decimal Discount { get; set; }
        public decimal priceAfterDiscount { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public List<mediaDto> GalleryUrls { get; set; }
        public productCategoryDto category { get; set; }
    }
}
