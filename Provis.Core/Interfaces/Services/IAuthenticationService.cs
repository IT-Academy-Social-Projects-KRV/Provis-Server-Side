using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task RegistrationAsync(User user, string password, string roleName);
        Task<UserTokensDTO> LoginAsync(string email, string password);
        Task<UserTokensDTO> RefreshTokenAsync(UserTokensDTO userTokensDTO);
    }
}
