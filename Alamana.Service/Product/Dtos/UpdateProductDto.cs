using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // وسائط (اختياري)
        //public IFormFile NewCover { get; set; }                // صورة غلاف جديدة
        public List<IFormFile> NewGallery { get; set; } = new();

        public List<int> RemoveMediaIds { get; set; } = new(); // IDs عناصر نزيلها
    }
}
