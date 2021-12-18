using Provis.Core.Interfaces.Services;
using System.Net;
using System.Net.Mail;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public Task SendAsync(string emailAddress, string message)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("admin@provis.com", "Provis");
            mailMessage.To.Add(emailAddress);
            mailMessage.Subject = "Weclom to Provis";
            mailMessage.Body = $"<div style=\"color: green;\">{message}</div>";

            using(SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Credentials = new NetworkCredential("herakrosisnews@gmail.com", "MyPassword_1");
                client.Port = 587;
                client.EnableSsl = true;
                client.Send(mailMessage);
            }

            return Task.CompletedTask;
        }
    }
}
