using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Service.Location.Dtos
{
    public class GetDestrictDto
    {
        public int Id { get; set; }
        public string district_name { get; set; }
        //public string NameAr { get; set; }
        public int CityId { get; set; }
        public city City { get; set; }
    }
}
