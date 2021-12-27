using Microsoft.Extensions.Options;
using MimeKit;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System.IO;
using Task = System.Threading.Tasks.Task;

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
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            email.Body = new TextPart(_mailSettings.TextFormat);

            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            await _smtpService.SendAsync(_mailSettings, email);
        }
    }
}
