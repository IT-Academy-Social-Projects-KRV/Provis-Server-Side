using Provis.Core.Interfaces.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class SmtpService : ISmtpService
    {
        public Task ConnectAsync(MailMessage mailMessage)
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Credentials = new NetworkCredential("herakrosisnews@gmail.com", "MyPassword_1"); // radi boga ne trogaite mou poshty thank you :)
                client.Port = 587;
                client.EnableSsl = true;
                client.Send(mailMessage);
            }

            return Task.CompletedTask;
        }
    }
}
