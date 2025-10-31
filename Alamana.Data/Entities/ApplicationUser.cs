using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace Alamana.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public CustomerType? CustomerType { get; set; } = 0;
    }
}
