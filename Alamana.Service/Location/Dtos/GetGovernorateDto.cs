using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Service.Location.Dtos
{
    public class GetGovernorateDto
    {
        public int Id { get; set; }
        public string governorate_name { get; set; }
        //public string NameAr { get; set; }

        public int CountryId { get; set; }
        public country Country { get; set; }
        public ICollection<city> Cities { get; set; }
    }
}
