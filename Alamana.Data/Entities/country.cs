using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class country : BaseEntity
    {
        public int Id { get; set; }
        public string country_name { get; set; }
        //public string country_name_ar { get; set; }
        public string country_code { get; set; }
        public string currency { get; set; }
        public ICollection<Governorate> Governorate { get; set; }
        //public ICollection<user_address> UserAddresses { get; set; }

    }
}
