using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.ContactUs.Dtos;

namespace Alamana.Service.ContactUs
{
    public interface IContactUsServices
    {
        Task SendContactMessageAsync(ContactUsDto dto);

    }
}
