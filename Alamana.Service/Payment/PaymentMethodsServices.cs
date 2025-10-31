using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Location.Dtos;
using Alamana.Service.Payment.Dtos;
using AutoMapper;

namespace Alamana.Service.Payment
{
    public class PaymentMethodsServices : IPaymentMethodsServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentMethodsServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IReadOnlyList<GetPaymentMethodsDto>> GetAllPaymentMethods()
        {
            var paymentMethods = await _unitOfWork.Repository<PaymentMethod>().GetAllAsync();
            var mappedPaymentMethods = _mapper.Map<IReadOnlyList<GetPaymentMethodsDto>>(paymentMethods);
            return mappedPaymentMethods;
        }




        public async Task<GetPaymentMethodsDto> GetPaymentMethodById(int id)
        {
            var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(id);
            var mappedPaymentMethod = _mapper.Map<GetPaymentMethodsDto>(paymentMethod);
            return mappedPaymentMethod;
        }
    }
}
