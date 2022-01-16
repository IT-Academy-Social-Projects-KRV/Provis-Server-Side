using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Helpers.Mails.ViewModels;
using Provis.Core.Interfaces.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class ConfirmEmailService : IConfirmEmailService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IEmailSenderService _emailService;
        protected readonly ITemplateService _templateService;
        protected readonly ClientUrl _clientUrl;

        public ConfirmEmailService(UserManager<User> userManager, 
            IEmailSenderService emailSender,
            ITemplateService templateService,
            IOptions<ClientUrl> options)
        {
            _userManager = userManager;
            _emailService = emailSender;
            _templateService = templateService;
            _clientUrl = options.Value;
        }

        public async Task SendConfirmMailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            CheckUserAndEmailConfirmed(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = Convert.ToBase64String(Encoding.Unicode.GetBytes(token));

            await _emailService.SendEmailAsync(new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Provis Confirm Email",
                Body = await _templateService.GetTemplateHtmlAsStringAsync("Mails/ConfirmEmail", 
                    new UserToken() { Token = encodedCode, UserName = user.UserName, Uri = _clientUrl.ApplicationUrl })
            });

            await Task.CompletedTask;
        }

        public async Task ConfirmEmailAsync(string userId, UserConfirmEmailDTO confirmEmailDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);

            CheckUserAndEmailConfirmed(user);

            var decodedCode = DecodeUnicodeBase64(confirmEmailDTO.ConfirmationCode);

            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if(!result.Succeeded)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Wrong code or this code is deprecated, try again!");
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await Task.CompletedTask;
        }

        private void CheckUserAndEmailConfirmed(User user)
        {
            user.UserNullChecking();

            if (user.EmailConfirmed)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You already confirmed your email address!");
            }
        }

        private string DecodeUnicodeBase64(string input)
        {
            var bytes = new Span<byte>(new byte[input.Length]);

            if(!Convert.TryFromBase64String(input, bytes, out var bytesWritten))
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid code, try again!");
            }

            return Encoding.Unicode.GetString(bytes.Slice(0, bytesWritten));
        }
    }
}
