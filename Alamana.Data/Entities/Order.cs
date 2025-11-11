using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Enums;

namespace Alamana.Data.Entities
{
    public class Order : BaseEntity
    {
        public int Id { get; set; }

        public string userId { get; set; }              // ✅ ضروري علشان نعرف مين صاحب الطلب
        public ApplicationUser user { get; set; }               // ✅ لو عندك كيان مستخدم
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public country Country { get; set; }
        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; }
        public int CityId { get; set; }
        public city City { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
        public string? Landmark { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        //public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }

}
