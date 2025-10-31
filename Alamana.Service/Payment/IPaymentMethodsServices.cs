using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Payment.Dtos;

namespace Alamana.Service.Payment
{
    public interface IPaymentMethodsServices
    {
        Task<IReadOnlyList<GetPaymentMethodsDto>> GetAllPaymentMethods();
        Task<GetPaymentMethodsDto> GetPaymentMethodById(int id);

    }
}
