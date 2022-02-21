using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Helpers;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    class JwtServiceTests
    {
        protected JwtService _jwtService;

        protected Mock<IOptions<JwtOptions>> _jwtOptionsMock;
        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IConfiguration> _configurationMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _configurationMock = new Mock<IConfiguration>();

            _jwtService = new JwtService(_jwtOptionsMock.Object, _userManagerMock.Object, _configurationMock.Object);
        }

        [Test]
        public async Task CreateRefreshToken_ValidToken_ReturnString()
        {
            var result = _jwtService.CreateRefreshToken();

            result.Should().NotBeNull();

            await Task.CompletedTask;
        }

        [Test]
        public async Task CreateToken_ValidToken_ReturnString()
        {
            var claims = GetClaims();

            SetupValue(new() 
            {
                Key = "minimumSixteenCharacters",
                Issuer = "issuer",
                LifeTime = 1000
            });

            var result = _jwtService.CreateToken(claims);

            result.Should().NotBeNull();

            await Task.CompletedTask;
        }

        [Test]
        public async Task GetClaimsFromExpiredToken_ValidToken_ReturnIEnumerableClaim()
        {
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjBkNDc5YzNmLWNjOWItNGYwNy05MDY1LWI2MGQ1MGEyYjBiZSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJPU2lkbCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE2NDUyODkwNDYsImlzcyI6IlByb3ZpcyBXZXAgQXBpIn0.VA1vlOUHGPQenmATSM3sIZm8oFJyfqoVPmeZEMWWZEg";
            var expectedClaims = new List<Claim>()
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    "0d479c3f-cc9b-4f07-9065-b60d50a2b0be",
                    "http://www.w3.org/2001/XMLSchema#string",
                    "Provis Wep Api",
                    "Provis Wep Api"),
                
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    "OSidl",
                    "http://www.w3.org/2001/XMLSchema#string",
                    "Provis Wep Api",
                    "Provis Wep Api"),

                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    "User",
                    "http://www.w3.org/2001/XMLSchema#string",
                    "Provis Wep Api",
                    "Provis Wep Api"),

                new Claim("exp",
                    "1645289046",
                    "http://www.w3.org/2001/XMLSchema#integer",
                    "Provis Wep Api",
                    "Provis Wep Api"),

                new Claim("iss",
                    "Provis Wep Api",
                    "http://www.w3.org/2001/XMLSchema#string",
                    "Provis Wep Api",
                    "Provis Wep Api"),
            };

            SetupValue(new()
            {
                Key = "SecretKey_12345!",
                Issuer = "Provis Wep Api",
                LifeTime = 1000
            });

            var result = _jwtService.GetClaimsFromExpiredToken(token);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedClaims);

            await Task.CompletedTask;
        }

        [Test]
        public async Task SetClaims_ValidToken_ReturnIEnumerableClaim()
        {
            var userMock = UserTestData.GetTestUser();
            var expectedClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userMock.Id),
                new Claim(ClaimTypes.Name, userMock.UserName),
            };

            SetupGetRolesAsync(userMock, new List<string>());

            var result = _jwtService.SetClaims(userMock);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedClaims);
            await Task.CompletedTask;
        }

        protected void SetupValue(JwtOptions jwtOptions)
        {
            _jwtOptionsMock
                .Setup(x => x.Value)
                .Returns(jwtOptions)
                .Verifiable();
        }

        protected void SetupGetRolesAsync(User user, IList<string> list)
        {
            _userManagerMock
                .Setup(x => x.GetRolesAsync(user).Result)
                .Returns(list)
                .Verifiable();
        }

        [TearDown]
        public void TearDown()
        {
            _jwtOptionsMock.Verify();
            _userManagerMock.Verify();
        }

        protected void SetupUserManagerUpdateAsync()
        {
            _userManagerMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(new IdentityResult())
                .Verifiable();
        }

        private IEnumerable<Claim> GetClaims()
        {
            return new List<Claim>()
            {
                new Claim("type", "value") { }
            };
        }
    }
}
