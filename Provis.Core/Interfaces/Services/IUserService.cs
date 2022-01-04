using Provis.Core.DTO.UserDTO;
using System.Threading.Tasks;
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
        Task UpdateUserImageAsync(IFormFile img, string userId);
        Task<DownloadFile> GetUserImageAsync(string userId);
    }
}
