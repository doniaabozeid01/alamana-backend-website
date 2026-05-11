using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Product.Dtos
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Weight { get; set; }
        public decimal Discount { get; set; }

        public bool New { get; set; }

        public List<IFormFile>? NewGallery { get; set; }

        public List<int>? RemoveMediaIds { get; set; }

        /// <summary>null = لا تغيّر التفاصيل. قائمة (حتى فارغة) = استبدال كامل.</summary>
        public List<ProductDetailFormItem>? Details { get; set; }

        /// <summary>
        /// إن وُجدت (حتى <c>[]</c>) تستبدل التفاصيل بالكامل؛ لها أولوية على <see cref="Details"/> إن كانت غير فارغة بعد التحليل.
        /// </summary>
        public string? DetailsJson { get; set; }
    }
}
