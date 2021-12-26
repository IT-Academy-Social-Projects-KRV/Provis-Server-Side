using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using System;
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
        public async Task SendConfirmMailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            CheckUserAndEmailConfirmed(user);

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

            CheckUserAndEmailConfirmed(user);

            var decodedCode = DecodeASCIIBase64(confirmEmailDTO.ConfirmCode);

            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if(result.Succeeded != true)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Wrong code or this code is deprecated, try again!");
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await Task.CompletedTask;
        }

        private void CheckUserAndEmailConfirmed(User user)
        {
            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "This user not found");
            }

            if (user.EmailConfirmed)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You already confirmed your email address!");
            }
        }

        private string DecodeASCIIBase64(string input)
        {
            var bytes = new Span<byte>(new byte[352]);

            if(!Convert.TryFromBase64String(input, bytes, out var bytesWritten))
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid code, try again!");
            }

            return Encoding.ASCII.GetString(bytes.Slice(0, bytesWritten));
        }
    }
}
