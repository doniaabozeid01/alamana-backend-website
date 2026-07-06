using System;
using System.Collections.Generic;
using Alamana.Service.Category.Dtos;

namespace Alamana.Service.Product.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal Price { get; set; }
        public bool New { get; set; }
        public decimal Discount { get; set; }
        public decimal priceAfterDiscount { get; set; }
        public string Weight { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public List<ProductDetailEntryDto> Details { get; set; } = new();
        public List<mediaDto> GalleryUrls { get; set; }
        public productCategoryDto category { get; set; }
    }
}
