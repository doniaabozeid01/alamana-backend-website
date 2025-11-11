using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.Orders.Dtos
{
    public class OrderRequest
    {
        public string UserId { get; set; } // ✅ إضافة UserId
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public int GovernorateId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }
        public string Landmark { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
