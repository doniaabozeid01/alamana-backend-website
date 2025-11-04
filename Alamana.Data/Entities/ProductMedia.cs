using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Enums;

namespace Alamana.Data.Entities
{
    public class ProductMedia : BaseEntity
    {
        public int Id { get; set; }
        public MediaType Type { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public Products Product { get; set; }
    }
}
