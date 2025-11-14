using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Video.Dtos
{
    public class CreateVideoDto
    {
        //public string Url { get; set; }
        public IFormFile? video { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
