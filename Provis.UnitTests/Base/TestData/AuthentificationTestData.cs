using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.UserEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Provis.UnitTests.Base.TestData
{
    public class AuthentificationTestData
    {
        public static List<Claim> GetClaimList()
        {
            return new List<Claim>()
            {
                new Claim("type", "value") {}
            };
        }

        public static UserTwoFactorDTO GetUserTwoFactorDTO()
        {
            return new UserTwoFactorDTO()
            {
                Email = "test1@gmail.com",
                Provider = "Email",
                Token = "token"
            };
        }
    }
}
