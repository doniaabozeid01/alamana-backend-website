using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Service.Email;
using Microsoft.Extensions.Configuration;

namespace Alamana.Service.ConfirmationEmail
{
    public class ConfirmationService : IConfirmationService
    {
        private readonly AlamanaBbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailSender _email;

        public ConfirmationService(AlamanaBbContext db, IConfiguration config, IEmailSender email)
        {
            _db = db; _config = config; _email = email;
        }

        public async Task SendOrRenewConfirmationAsync(string userId, string email, string actionType)
        {
            // امسحي أي Pending قديم لنفس المستخدم/الأكشن
            var oldPendings = _db.EmailConfirmationRequests
                .Where(x => x.UserId == userId && x.ActionType == actionType && x.IsConfirmed == null);
            _db.EmailConfirmationRequests.RemoveRange(oldPendings);
            await _db.SaveChangesAsync();

            var req = new EmailConfirmationRequest
            {
                UserId = userId,
                Email = email,
                ActionType = actionType,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
            _db.EmailConfirmationRequests.Add(req);
            await _db.SaveChangesAsync();

            var baseUrl = "https://bubblehope.com";

            // اللينكات اللي تروح على الفرونت (Angular)
            var confirmUrl = $"{baseUrl}/confirm-email/{req.Id}";
            var rejectUrl = $"{baseUrl}/reject-email/{req.Id}";


            var subject = "Confirm your email";
            var body = $@"
<p>We received a request to {(actionType == "login" ? "log in" : "activate your account")} using this email.</p>
<p>Please choose:</p>
<p>
  <a href=""{confirmUrl}"">Accept</a> &nbsp; | &nbsp;
  <a href=""{rejectUrl}"">Reject</a>
</p>
<p>This link expires in 1 hour.</p>";

            await _email.SendEmailAsync(email, subject, body);
        }
    }

}
