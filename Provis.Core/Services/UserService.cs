using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Helpers.Mails;

namespace Provis.Core.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IEmailSenderService _emailSenderService;
        protected readonly IMapper _mapper;

        public UserService(UserManager<User> userManager,
            IRepository<User> userRepository,
            IRepository<InviteUser> inviteUser,
            IMapper mapper,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _inviteUserRepository = inviteUser;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
        }

        public async Task<UserPersonalInfoDTO> GetUserPersonalInfoAsync(string userId)
        {
            var user = await _userRepository.GetByKeyAsync(userId);

            if(user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var userPersonalInfo = _mapper.Map<UserPersonalInfoDTO>(user);

            return userPersonalInfo;
        }

        public async Task ChangeInfoAsync(string userId, UserChangeInfoDTO userChangeInfoDTO)
        {
            var userObject = await _userManager.FindByNameAsync(userChangeInfoDTO.UserName);

            if (userObject != null && userObject.Id != userId)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, 
                    "This username already exists");
            }

            var user = await _userRepository.GetByKeyAsync(userId);

            _mapper.Map(userChangeInfoDTO, user);

            await _userRepository.UpdateAsync(user);

            await _userManager.UpdateNormalizedUserNameAsync(user);

            await _userRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }
        
        public async Task<List<UserInviteInfoDTO>> GetUserInviteInfoListAsync(string userId)
        {
            var user = await _userRepository.GetByKeyAsync(userId);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var inviteListInfo = await _inviteUserRepository.Query()
                .Where(u => u.ToUserId == userId).Include(w => w.Workspace).Include(u => u.FromUser)
                .OrderBy(d => d.Date ).ToListAsync();

            var userInviteListInfoToReturn = _mapper.Map<List<UserInviteInfoDTO>>(inviteListInfo);
            
           return userInviteListInfoToReturn;
        }

        public async Task<UserActiveInviteDTO> IsActiveInviteAsync(string userId)
        {
            var user = await _userRepository.GetByKeyAsync(userId);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");

            }
            var userActiveInviteDTO = new UserActiveInviteDTO();

            userActiveInviteDTO.IsActiveInvite = await _inviteUserRepository.Query()
                .AnyAsync(u => u.ToUserId == userId && u.IsConfirm == null);

            return userActiveInviteDTO;
        }

        public async Task<bool> CheckIsTwoFactorVerificationAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "User with Id not exist");
            }

            if (!user.EmailConfirmed)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                    "First you need to confirm your email address");
            }

            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        public async Task ChangeTwoFactorVerificationStatusAsync(string userId, UserChange2faStatusDTO statusDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "User with Id not exist");
            }

            var isUserToken = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", statusDTO.Token);

            if(!isUserToken)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid code");
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, !await _userManager.GetTwoFactorEnabledAsync(user));

            if(!result.Succeeded)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid request");
            }

            await Task.CompletedTask;
        }

        public async Task SendTwoFactorCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "User with Id not exist");
            }

            var twoFactorToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var message = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Provis 2fa code",
                Body = $"<div><h1>Your code:</h1> <label>{twoFactorToken}</label></div>"
            };

            await _emailSenderService.SendEmailAsync(message);

            await Task.CompletedTask;
        }
    }
}
