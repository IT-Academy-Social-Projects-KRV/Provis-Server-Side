using Provis.Core.Helpers.Mails;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
