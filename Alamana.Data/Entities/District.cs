using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class District : BaseEntity
    {
        public int Id { get; set; }
        public string district_name { get; set; }
        //public string NameAr { get; set; }
        public int CityId { get; set; }
        public city City { get; set; }
    }

}
