using Provis.Core.Helpers.Mails;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(MailRequest mailRequest);

        Task SendManyMailsAsync<T>(MailingRequest<T> mailing) where T : class, new();
    }
}
