using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserPersonalInfoDTO> GetUserPersonalInfoAsync(string userId);
        Task ChangeNameAsync(string userId, UserChangeNameDTO userChangeNameDTO);
        Task ChangeSurnameAsync(string userId, UserChangeSurnameDTO userChangeSurnameDTO);
        Task ChangeUsernameAsync(string userId, UserChangeUsernameDTO userChangeNameDTO);
    }
}
