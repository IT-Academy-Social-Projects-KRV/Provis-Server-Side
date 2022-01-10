using Provis.Core.Helpers.Mails;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
