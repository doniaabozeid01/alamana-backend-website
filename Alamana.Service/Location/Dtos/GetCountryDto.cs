using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Service.Location.Dtos
{
    public class GetCountryDto
    {
        public int Id { get; set; }
        public string country_name { get; set; }
        //public string country_name_ar { get; set; }
        public string country_code { get; set; }
        public string currency_en { get; set; }
        public string currency_ar { get; set; }
        public string? office_address { get; set; }
        public string? phone { get; set; }
        public string? phone2 { get; set; }
        public string? email { get; set; }
        public string? working_hours { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<Governorate> Governorate { get; set; }
        //public ICollection<user_address> UserAddresses { get; set; }
    }
}
