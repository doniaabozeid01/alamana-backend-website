using System.Collections.Generic;
using Alamana.Service.Product.Dtos;

namespace Alamana.Service.Category.Dtos
{
    public class CategoryWithProductsDto
    {
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
