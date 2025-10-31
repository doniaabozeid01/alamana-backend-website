using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.Email
{
    public interface IEmailSender
    {
        //Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailAsync(
                    string toEmail,
                    string subject,
                    string body,
                    string? replyToEmail = null,
                    string? replyToName = null);
    }
}
