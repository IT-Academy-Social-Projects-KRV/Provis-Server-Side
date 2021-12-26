﻿using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System;
using System.Text;
using System.Web;
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
        public async Task SendConfirmMailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));

            await _emailService.SendEmailAsync(new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Provis Confirm Email",
                Body = $"Your code: {encodedCode}"
            });

            await Task.CompletedTask;
        }

        public async Task ConfirmEmailAsync(string userId, UserConfirmEmailDTO confirmEmailDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var decodedCode = Encoding.ASCII.GetString(Convert.FromBase64String(confirmEmailDTO.ConfirmCode));

            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if(result.Succeeded != true)
            {
                throw new HttpException(System.Net.HttpStatusCode.NoContent, "Error");
            }

            await Task.CompletedTask;
        }
    }
}
