using System.Net.Mail;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ISmtpService
    {
        Task ConnectAsync(MailMessage mailMessage);
    }
}
