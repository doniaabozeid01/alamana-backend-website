using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class FavouriteProducts : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public Products Product { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
    }
}
