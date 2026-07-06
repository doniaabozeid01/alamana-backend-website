using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Product.Dtos
{
    public class UpdateProductDto
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int CategoryId { get; set; }
        public int CountryId { get; set; }
        public decimal Price { get; set; }
        public string Weight { get; set; }
        public decimal Discount { get; set; }
        public bool New { get; set; }
        public int Stock { get; set; }

        public List<IFormFile>? NewGallery { get; set; }

        public List<int>? RemoveMediaIds { get; set; }

        public List<ProductDetailFormItem>? Details { get; set; }

        public string? DetailsJson { get; set; }
    }
}
