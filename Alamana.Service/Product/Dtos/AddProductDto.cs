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

        /// <summary>أقسام المنتج (key / value / order) — اختياري.</summary>
        public List<ProductDetailFormItem>? Details { get; set; }

        /// <summary>
        /// اختياري: مصفوفة JSON واحدة مثل
        /// <c>[{"key":"وصف","value":"نص","sortOrder":1}]</c>
        /// لها أولوية على <see cref="Details"/> إذا كانت غير فارغة بعد التحليل.
        /// </summary>
        public string? DetailsJson { get; set; }
    }
}
