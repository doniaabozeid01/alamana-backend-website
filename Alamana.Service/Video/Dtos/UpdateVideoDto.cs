using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.Video.Dtos
{
    public class UpdateVideoDto
    {
        public string Url { get; set; }
        public bool IsDefault { get; set; }
    }
}
