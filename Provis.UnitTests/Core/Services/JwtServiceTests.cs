using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Helpers;
using Provis.Core.Services;
using Provis.UnitTests.Base;
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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
            _userManagerMock = UserManagerMock.GetUserManager<User>();

            _jwtService = new JwtService(_jwtOptionsMock.Object, _userManagerMock.Object);
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

            SetupKey(new() 
            {
                Key = "minimumSixteenCharacters",
                Issuer = "issuer",
                LifeTime = 1000
            });

            var result = _jwtService.CreateToken(claims);

            result.Should().NotBeNull();

            await Task.CompletedTask;
        }

        protected void SetupKey(JwtOptions jwtOptions)
        {
            _jwtOptionsMock
                .Setup(x => x.Value)
                .Returns(jwtOptions)
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
