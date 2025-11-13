using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class Videos : BaseEntity
    {
        public int Id { get; set; }
          public string  Url { get; set; }
          public bool  IsDefault { get; set; }
    }
}
