using Microsoft.Extensions.Options;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using SendGrid;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly Helpers.Mails.MailSettings _mailSettings;
        public EmailSenderService(IOptions<Helpers.Mails.MailSettings> options)
        {
            _mailSettings = options.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var client = new SendGridClient(_mailSettings.ApiKey);

            await client.SendEmailAsync(CreateMessage(mailRequest));
        }

        private SendGridMessage CreateMessage(MailRequest mailRequest)
        {
            SendGridMessage message = new SendGridMessage
            {
                From = new EmailAddress(_mailSettings.Email, _mailSettings.DisplayName),
                Subject = mailRequest.Subject,
                HtmlContent = mailRequest.Body,
            };

            message.AddTo(mailRequest.ToEmail);

            return message;
        }
    }
}
