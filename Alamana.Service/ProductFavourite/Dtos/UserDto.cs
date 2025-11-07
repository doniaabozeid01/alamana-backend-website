using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.ProductFavourite.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }             // أو int حسب نوع معرف المستخدم
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
