using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Data.Entities
{
    public class EmailConfirmationRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public bool? IsConfirmed { get; set; } // null = لسه ما استعملش اللينك، true = confirmed، false = rejected
        public string ActionType { get; set; } = "login"; // login أو register (اختياري)
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(5);
    }
}
