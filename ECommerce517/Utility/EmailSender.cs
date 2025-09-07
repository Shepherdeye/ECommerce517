

using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ECommerce517.Utility
{
    public class EmailSender : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ahmedgamil288@gmail.com", "dxdp oons esty iiii")
            };
            return client.SendMailAsync(
           new MailMessage(from: "your.email@live.com",
                           to: email,
                           subject,
                           htmlMessage
                           )
           {
               IsBodyHtml = true
           });

        }
    }
}
