using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Provis.Core.DTO.UserDTO;
using Microsoft.AspNetCore.Http;
using Provis.Core.ApiModels;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserPersonalInfoDTO> GetUserPersonalInfoAsync(string userId);
        Task ChangeInfoAsync(string userId, UserChangeInfoDTO userChangeInfoDTO);
        Task<List<UserInviteInfoDTO>> GetUserInviteInfoListAsync(string userId);
        Task<UserActiveInviteDTO> IsActiveInviteAsync(string userId);
        Task ChangeTwoFactorVerificationStatusAsync(string userId, UserChange2faStatusDTO statusDTO);
        Task<bool> CheckIsTwoFactorVerificationAsync(string userId);
        Task SendTwoFactorCodeAsync(string userId);
        Task UpdateUserImageAsync(IFormFile img, string userId);
        Task<DownloadFile> GetUserImageAsync(string userId);
    }
}
