using Provis.Core.Interfaces.Services;
using System.Net.Mail;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ISmtpService _smtpService;
        public EmailSenderService(ISmtpService smtpService)
        {
            _smtpService = smtpService;
        }

        public async Task SendAsync(string emailAddress, string message)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("admin@provis.com", "Provis");
            mailMessage.To.Add(emailAddress);
            mailMessage.Subject = "Weclom to Provis";
            mailMessage.Body = $"<div style=\"color: green;\">{message}</div>";

            await _smtpService.ConnectAsync(mailMessage);
        }
    }
}
