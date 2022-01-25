using Microsoft.Extensions.Options;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

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

        public async Task<Response> SendEmailAsync(SendGridMessage message)
        {
            var client = new SendGridClient(_mailSettings.ApiKey);

            return await client.SendEmailAsync(message);
        }

        public async Task SendManyMailsAsync<T>(MailingRequest<T> mailing) where T : class, new()
        {
            var emailBody = await _templateService.GetTemplateHtmlAsStringAsync(
                $"Mails/{mailing.Body}", mailing.ViewModel);

            var tokenSource = new CancellationTokenSource();

            var listOfList = new List<List<string>>();

            for(int i = 0; i <= mailing.Emails.Count() / _mailSettings.MailingGroup; i++)
            {
                listOfList.Add(mailing.Emails.Skip(_mailSettings.MailingGroup * i)
                    .Take(_mailSettings.MailingGroup).ToList());
            }

            List<Task<List<Response>>> tasks = new();
            foreach(var list in listOfList)
            {
                tasks.Add(Task<List<Response>>.Factory.StartNew(() =>
                {
                    return SendMailingGroup(list, emailBody, mailing.Subject).GetAwaiter().GetResult();
                }, 
                tokenSource.Token));
            }

            Task.WaitAll(tasks.ToArray()); 
        }

        private async Task<List<Response>> SendMailingGroup(IEnumerable<string> emails, 
            string emailBody, string subject)
        {
            List<Response> responses = new();
            foreach (var userEmail in emails)
            {
                responses.Add(await SendEmailAsync(CreateMessage(new MailRequest()
                {
                    ToEmail = userEmail,
                    Subject = subject,
                    Body = emailBody
                })));
            }

            return responses;
        }

        private SendGridMessage CreateMessage(MailRequest mailRequest)
        {
            SendGridMessage message = new()
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
