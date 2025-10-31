using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Security;
using MimeKit;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace Alamana.Service.Email
{
    public class EmailSender : IEmailSender
    {


        //private readonly string _smtpServer = "smtp.gmail.com";
        //private readonly string _smtpUser = "alhndal212@gmail.com";  // هنا ضع بريدك الإلكتروني
        //private readonly string _smtpPass = "fefp iibu uhbm vllr";  // هنا ضع كلمة مرورك (أو كلمة مرور التطبيق إذا كنت تستخدم التوثيق الثنائي)
        ////private readonly string _smtpPass = "ubur gqjm rpyh ghrq";  // هنا ضع كلمة مرورك (أو كلمة مرور التطبيق إذا كنت تستخدم التوثيق الثنائي)
        //private readonly int _smtpPort = 587;  // المنفذ الصحيح لـ STARTTLS


        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress("Al-Hendal", _smtpUser));
        //    emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
        //    emailMessage.Subject = subject;

        //    // استخدام التوقيت المحلي بدلًا من UTC
        //    //var localTime = DateTimeOffset.Now;  // توقيت محلي
        //    //emailMessage.Date = localTime;

        //    var bodyBuilder = new BodyBuilder { HtmlBody = body };
        //    emailMessage.Body = bodyBuilder.ToMessageBody();

        //    using (var smtpClient = new SmtpClient())
        //    {
        //        await smtpClient.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
        //        await smtpClient.AuthenticateAsync(_smtpUser, _smtpPass);
        //        await smtpClient.SendAsync(emailMessage);
        //        await smtpClient.DisconnectAsync(true);
        //    }

        //}
















        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly string _smtpUser = "alhndal212@gmail.com";  // هنا ضع بريدك الإلكتروني
        private readonly string _smtpPass = "fefp iibu uhbm vllr";  // هنا ضع كلمة مرورك (أو كلمة مرور التطبيق إذا كنت تستخدم التوثيق الثنائي)
        private readonly int _smtpPort = 587;

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            string? replyToEmail = null,
            string? replyToName = null)
        {
            var emailMessage = new MimeMessage();

            // 👇 from = الإيميل الرسمي بتاعك
            emailMessage.From.Add(new MailboxAddress("Alamana", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("Recipient", toEmail));
            emailMessage.Subject = subject;



            // ✅ هنا السطر المهم:
            if (!string.IsNullOrEmpty(replyToEmail))
            {
                emailMessage.ReplyTo.Add(new MailboxAddress(replyToName ?? replyToEmail, replyToEmail));
            }

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_smtpUser, _smtpPass);
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
        }






    }
}
