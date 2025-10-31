using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Advertisements : BaseEntity
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; } // رابط الصورة
        public string Title { get; set; }
        //public string TitleAr { get; set; }
        public string Description { get; set; }
        //public string DescriptionAr { get; set; }
    }
}
