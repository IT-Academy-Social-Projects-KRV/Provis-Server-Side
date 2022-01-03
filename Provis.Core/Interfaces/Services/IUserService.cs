using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserPersonalInfoDTO> GetUserPersonalInfoAsync(string userId);
        Task ChangeInfoAsync(string userId, UserChangeInfoDTO userChangeInfoDTO);
        Task<List<UserInviteInfoDTO>> GetUserInviteInfoListAsync(string userId);
        Task<UserActiveInviteDTO> IsActiveInviteAsync(string userId);
        Task ChangeTwoFactorAuthentication(string userId, UserChangeTwoFactorDTO factorDTO);
    }   
}
