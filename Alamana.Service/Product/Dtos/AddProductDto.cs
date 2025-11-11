using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        //public IFormFile ImagePathCover { get; set; }
        public List<IFormFile> Gallery { get; set; } = new();  

        public int CategoryId { get; set; }
    }
}
