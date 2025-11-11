using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.ConfirmationEmail
{
    public interface IConfirmationService
    {
        Task SendOrRenewConfirmationAsync(string userId, string email, string actionType);

    }
}
