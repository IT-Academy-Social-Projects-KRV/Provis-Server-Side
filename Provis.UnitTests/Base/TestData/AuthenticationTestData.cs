using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Provis.UnitTests.Base.TestData
{
    public class AuthenticationTestData
    {
        public static UserExternalAuthDTO GetUserAuthDTO()
        {
            return new UserExternalAuthDTO()
            {
                IdToken = "1",
                Provider = "2"
            };
        }

        public static GoogleJsonWebSignature.Payload GetPayload()
        {
            return new GoogleJsonWebSignature.Payload()
            {
                Scope = "1",
                Prn = "1",
                HostedDomain = "1",
                Email = "test1@gmail.com",
                EmailVerified = true,
                Name = "Name1",
                GivenName = "Name1",
                FamilyName = "Name1",
                Picture = "1",
                Locale = "1",
                Subject = "1"
            };
        }

        public static UserLoginInfo GetUserLoginInfo()
        {
            return new UserLoginInfo("2","1","2")
            {
                LoginProvider = "2",
                ProviderKey = "1",
                ProviderDisplayName = "2"
            };
        }

        public static IEnumerable<Claim> GetClaims()
        {
            return new List<Claim>
            {
                new Claim("1","2"),
                new Claim("3","4")
            };
        }
    }
}