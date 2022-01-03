using MimeKit;
using Provis.Core.Helpers.Mails;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ISmtpService
    {
        Task SendAsync(MimeMessage email, MailSettings mailSettings);
    }
}
