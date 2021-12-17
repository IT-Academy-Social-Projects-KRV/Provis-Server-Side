using Provis.Core.DTO.userDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserPersonalInfoDTO> GetUserPersonalInfoAsync(string userId);
    }
}
