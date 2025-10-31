using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.ContactUs.Dtos;
using Alamana.Service.Email;

namespace Alamana.Service.ContactUs
{
    public class ContactUsServices : IContactUsServices
    {
        private readonly IEmailSender _emailSender;

        public ContactUsServices(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendContactMessageAsync(ContactUsDto dto)
        {
            var subject = $"رسالة جديدة من {dto.FullName}";
            var body = $@"
            <p><strong>الاسم:</strong> {dto.FullName}</p>
            <p><strong>البريد:</strong> {dto.Email}</p>
            <p><strong>الرسالة:</strong><br/>{dto.Message}</p>
        ";

            await _emailSender.SendEmailAsync(
             toEmail: "alhendal01@gmail.com",
             subject: $"رسالة جديدة من {dto.FullName}",
             body: $@"
                <p><strong>الاسم:</strong> {dto.FullName}</p>
                <p><strong>البريد:</strong> {dto.Email}</p>
                <p><strong>الرسالة:</strong><br/>{dto.Message}</p>
            ",
             replyToEmail: dto.Email,
             replyToName: dto.FullName
            );

        }
    }
}
