using Microsoft.AspNetCore.Identity;
using Provis.Core.Entities;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class ConfirmEmailService : IConfirmEmailService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSenderService _emailService;
        public ConfirmEmailService(UserManager<User> userManager, IEmailSenderService emailSender)
        {
            _userManager = userManager;
            _emailService = emailSender;
        }
        public async Task SendConfirmMailAsync(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailService.SendEmailAsync(new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Provis Confirm Email",
                Body = $"Your code: {code}"
            });

            await Task.CompletedTask;
        }
    }
}
