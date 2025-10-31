using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using AutoMapper;

namespace Alamana.Service.Payment.Dtos
{
    public class paymentMethodsProfile : Profile
    {
        public paymentMethodsProfile()
        {
            CreateMap<PaymentMethod, GetPaymentMethodsDto>();
        }
    }
}
