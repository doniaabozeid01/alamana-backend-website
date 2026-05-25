using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Product.Dtos
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public bool New { get; set; }
        public decimal Discount { get; set; }

        public List<IFormFile>? Gallery { get; set; }

        public int CategoryId { get; set; }

        /// <summary>
        /// تفاصيل المنتج — اختياري. من Angular/Postman مع الصور: <c>Details[0].Key</c>, <c>Details[0].Value</c>, <c>Details[0].SortOrder</c>.
        /// </summary>
        public List<ProductDetailFormItem>? Details { get; set; }

        /// <summary>
        /// اختياري: مصفوفة JSON واحدة؛ لها أولوية على <see cref="Details"/> إن وُجدت وصالحة.
        /// تجنّبي ترك القيمة الافتراضية <c>string</c> في Swagger.
        /// </summary>
        //public string? DetailsJson { get; set; }
    }
}
