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
    }
}
