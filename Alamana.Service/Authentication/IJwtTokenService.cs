using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Service.Authentication
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(ApplicationUser user);

    }
}
