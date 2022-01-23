using Provis.Core.Helpers.Mails;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(MailRequest mailRequest);

        Task<Response> SendEmailAsync(SendGridMessage message);

        Task SendManyMailsAsync<T>(MailingRequest<T> mailing) where T : class, new();
    }
}
