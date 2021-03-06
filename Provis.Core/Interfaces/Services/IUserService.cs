using System.Threading.Tasks;
using Provis.Core.DTO.UserDTO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Provis.Core.ApiModels;

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
        Task SetPasswordAsync(string userId, UserSetPasswordDTO userSetPasswordDTO);
        Task<bool> IsHavePasswordAsync(string userId);
    }
}
