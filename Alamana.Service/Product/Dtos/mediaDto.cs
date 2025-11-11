using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Enums;

namespace Alamana.Service.Product.Dtos
{
    public class mediaDto
    {
        public MediaType Type { get; set; }
        public string Url { get; set; }
    }
}
