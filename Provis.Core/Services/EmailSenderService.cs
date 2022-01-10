using Microsoft.Extensions.Options;
using MimeKit;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly MailSettings _mailSettings;
        private readonly ISmtpService _smtpService;
        public EmailSenderService(IOptions<MailSettings> options, ISmtpService smtpService)
        {
            _mailSettings = options.Value;
            _smtpService = smtpService;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            await _smtpService.SendAsync(CreateEmailMessage(mailRequest), _mailSettings);
        }

        private MimeMessage CreateEmailMessage(MailRequest mailRequest)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddress));
            emailMessage.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            emailMessage.Subject = mailRequest.Subject;
            emailMessage.Body = new TextPart(_mailSettings.TextFormat) { Text = mailRequest.Body };

            return emailMessage;
        }
    }
}
