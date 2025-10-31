using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.Advertisment.Dtos
{
    public class AdvertiseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        //public string TitleAr { get; set; }
        public string Description { get; set; }
        //public string DescriptionAr { get; set; }
    }

}
