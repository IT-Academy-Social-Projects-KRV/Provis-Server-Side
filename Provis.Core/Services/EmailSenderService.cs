using Provis.Core.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public void Send(string emailAddress, string message, string userName)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("admin@provis.com", "Provis");
            mailMessage.To.Add(emailAddress);
            mailMessage.Subject = "Weclom to Provis";
            mailMessage.Body = "<div style=\"color: green;\">Hello from Provis</div>";

            using(SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Credentials = new NetworkCredential("herakrosisnews@gmail.com", "");
                client.Port = 587;
                client.EnableSsl = true;
                client.Send(mailMessage);
            }
        }
    }
}
