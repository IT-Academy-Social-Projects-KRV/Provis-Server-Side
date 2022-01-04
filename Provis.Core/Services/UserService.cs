using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Provis.Core.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Provis.Core.ApiModels;

namespace Provis.Core.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IOptions<ImageSettings> _imageSettings;

        public UserService(UserManager<User> userManager,
            IRepository<User> userRepository,
            IRepository<InviteUser> inviteUser,
            IMapper mapper,
            IFileService fileService,
            IOptions<ImageSettings> imageSettings)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _inviteUserRepository = inviteUser;
            _mapper = mapper;
            _fileService = fileService;
            _imageSettings = imageSettings;
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
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "This username already exists");
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

            var inviteListInfo = await _inviteUserRepository.Query().Where(u => u.ToUserId == userId).Include(w => w.Workspace).Include(u => u.FromUser).OrderBy(d => d.Date ).ToListAsync();

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

            userActiveInviteDTO.IsActiveInvite = await _inviteUserRepository.Query().AnyAsync(u => u.ToUserId == userId && u.IsConfirm == null);

            return userActiveInviteDTO;
        }

        public async Task UpdateUserImageAsync(IFormFile img, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            string newPath = await _fileService.AddFileAsync(img.OpenReadStream(), _imageSettings.Value.Path, img.FileName);

            if (user.Img != null)
            {
                await _fileService.DeleteFileAsync(user.Img);
            }

            user.Img = newPath;

            await _userManager.UpdateAsync(user);
        }

        public async Task<DownloadFile> GetUserImageAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            _ = user.Img ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Image not found");

            var file = await _fileService.GetFileAsync(user.Img);

            return file;
        }
    }
}
