using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class city : BaseEntity
    {
        public int Id { get; set; }
        public string city_name { get; set; }
        //public string city_name_ar { get; set; }
        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; }
        public ICollection<District> Districts { get; set; }
    }
}
