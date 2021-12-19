using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class SmtpService : ISmtpService
    {
        public async Task SendAsync(MailSettings mailSettings, MimeMessage email)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.CheckCertificateRevocation = false;
                await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                //smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                smtp.Authenticate(mailSettings.Email, mailSettings.Password);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
        }
    }
}
