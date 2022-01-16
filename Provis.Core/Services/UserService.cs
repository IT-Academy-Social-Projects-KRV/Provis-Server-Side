using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.UserDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provis.Core.ApiModels;
using Provis.Core.Exeptions;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Helpers.Mails.ViewModels;

namespace Provis.Core.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IEmailSenderService _emailSenderService;
        protected readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IOptions<ImageSettings> _imageSettings;
        protected readonly ITemplateService _templateService;


        public UserService(UserManager<User> userManager,
            IRepository<User> userRepository,
            IRepository<InviteUser> inviteUser,
            IMapper mapper,
            IEmailSenderService emailSenderService,
            IFileService fileService,
            IOptions<ImageSettings> imageSettings,
            ITemplateService templateService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _inviteUserRepository = inviteUser;
            _mapper = mapper;
            _fileService = fileService;
            _imageSettings = imageSettings;
            _emailSenderService = emailSenderService;
            _templateService = templateService;
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

            var specification = new InviteUsers.InviteList(userId);
            var inviteListInfo = await _inviteUserRepository.GetListBySpecAsync(specification);

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

            var specification = new InviteUsers.ActiveInvites(userId);
            userActiveInviteDTO.IsActiveInvite = await _inviteUserRepository.AnyBySpecAsync(specification);

            return userActiveInviteDTO;
        }

        public async Task UpdateUserImageAsync(IFormFile img, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            string newPath = await _fileService.AddFileAsync(img.OpenReadStream(), _imageSettings.Value.Path, img.FileName);

            if (user.ImageAvatarUrl != null)
            {
                await _fileService.DeleteFileAsync(user.ImageAvatarUrl);
            }

            user.ImageAvatarUrl = newPath;

            await _userManager.UpdateAsync(user);
        }

        public async Task<DownloadFile> GetUserImageAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            _ = user.ImageAvatarUrl ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Image not found");

            var file = await _fileService.GetFileAsync(user.ImageAvatarUrl);

            return file;
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

            if (!isUserToken)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid code");
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, !await _userManager.GetTwoFactorEnabledAsync(user));

            if (!result.Succeeded)
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
                Body = await _templateService.GetTemplateHtmlAsStringAsync("Mails/TwoFactorCode", 
                    new UserToken() { Token = twoFactorToken, UserName = user.UserName })
            };

            await _emailSenderService.SendEmailAsync(message);

            await Task.CompletedTask;
        }
    }
}
