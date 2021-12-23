using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Provis.Core.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IMapper _mapper;

        public UserService(UserManager<User> userManager,
            IRepository<User> userRepository,
            IRepository<InviteUser> inviteUser,
            IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _inviteUserRepository = inviteUser;
            _mapper = mapper;
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
    }
}
