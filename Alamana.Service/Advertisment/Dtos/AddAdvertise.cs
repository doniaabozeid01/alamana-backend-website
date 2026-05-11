using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Advertisment.Dtos
{
    public class AddAdvertise
    {
        public IFormFile? ImageUrl { get; set; }
        public string Title { get; set; }
        //public string TitleAr { get; set; }
        public string Description { get; set; }
        //public string DescriptionAr { get; set; }

        /// <summary>معرّفات المنتجات المرتبطة بالإعلان (اختياري). في multipart: ProductIds=1&amp;ProductIds=2</summary>
        public List<int>? ProductIds { get; set; }
    }
}
