using Microsoft.Extensions.Options;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ITemplateService _templateService;
        private readonly Helpers.Mails.MailSettings _mailSettings;
        public EmailSenderService(IOptions<Helpers.Mails.MailSettings> options,
            ITemplateService templateService)
        {
            _mailSettings = options.Value;
            _templateService = templateService;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var client = new SendGridClient(_mailSettings.ApiKey);

            await client.SendEmailAsync(CreateMessage(mailRequest));
        }

        public async Task SendManyMailsAsync<T>(MailingRequest<T> mailing) where T : class, new()
        {
            var emailBody = await _templateService.GetTemplateHtmlAsStringAsync(
                $"Mails/{mailing.Body}", mailing.ViewModel);

            await Task.Factory.StartNew(async () =>
            {
                foreach (var userEmail in mailing.Emails)
                {
                    await SendEmailAsync(new MailRequest()
                    {
                        ToEmail = userEmail,
                        Subject = mailing.Subject,
                        Body = emailBody
                    });
                }
            }); 
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
