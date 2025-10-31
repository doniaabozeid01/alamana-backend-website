using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Governorate : BaseEntity
    {
        public int Id { get; set; }
        public string governorate_name { get; set; }
        //public string NameAr { get; set; }

        public int CountryId { get; set; }
        public country Country { get; set; }
        public ICollection<city> Cities { get; set; }

    }
}
