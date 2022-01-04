using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task RegistrationAsync(User user, string password, string roleName);
        Task<UserTokensDTO> LoginAsync(string email, string password);
        Task<UserTokensDTO> RefreshTokenAsync(UserTokensDTO userTokensDTO);
        Task LogoutAsync(UserTokensDTO userTokensDTO);
    }
}
