using FluentAssertions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.RefreshTokenEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using Provis.UnitTests.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    class AuthentificationServiceTests
    {
        protected AuthenticationService _authentifiactioService;

        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<SignInManager<User>> _signInManagerMock;
        protected Mock<IJwtService> _jwtServiceMock;
        protected Mock<RoleManager<IdentityRole>> _roleManagerMock;
        protected Mock<IRepository<RefreshToken>> _refreshTokenRepositoryMock;
        protected Mock<IEmailSenderService> _emailSenderServiceMock;
        protected Mock<IConfirmEmailService> _confirmEmailServiceMock;
        protected Mock<ITemplateService> _templateServiceMock;
        protected Mock<IOptions<ClientUrl>> _clientUrlMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            try
            {
                _userManagerMock = UserManagerMock.GetUserManager<User>();
                _signInManagerMock = SignInManagerMock.GetSignInManager<User>();
                _roleManagerMock = RoleManagerMock.GetRoleManager<IdentityRole>();

                _jwtServiceMock = new Mock<IJwtService>();
                _refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken>>();
                _emailSenderServiceMock = new Mock<IEmailSenderService>();
                _confirmEmailServiceMock = new Mock<IConfirmEmailService>();
                _templateServiceMock = new Mock<ITemplateService>();
                _clientUrlMock = new Mock<IOptions<ClientUrl>>();

                _authentifiactioService = new AuthenticationService(
                    _userManagerMock.Object,
                    _signInManagerMock.Object,
                    _jwtServiceMock.Object,
                    _roleManagerMock.Object,
                    _refreshTokenRepositoryMock.Object,
                    _emailSenderServiceMock.Object,
                    _confirmEmailServiceMock.Object,
                    _templateServiceMock.Object,
                    _clientUrlMock.Object);
            }
            catch(Exception ex)
            {

            }
        }

        [Test]
        [TestCase("test1@gmail.com")]
        public void LoginAsync_UserExist_ReturnUserAutorizationDTO(string email)
        {
            //var userMock = UserTestData.GetTestUser();
            //var expectedUser = new UserPersonalInfoDTO()
            //{
            //    Name = userMock.Name,
            //    Surname = userMock.Surname,
            //    Email = userMock.Email,
            //    Username = userMock.UserName
            //};

            //SetupFindByEmailAsync(email, userMock);

            //SetupCheckPasswordAsync(userMock, "Password_1");

        }

        [Test]
        public async Task ExternalLoginAsync_PayloadIsInvalid_ThrowHttpException()
        {
            var authDTOMock = AuthenticationTestData.GetUserAuthDTO();

            Func<Task> act = () =>
                _authentifiactioService.ExternalLoginAsync(authDTOMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InvalidRequest);
        }

        [Test]
        public async Task ExternalLoginAsync_PayloadIsValid_ReturnUserAuthResponseDTO()
        {
            var userAuthDTO = AuthenticationTestData.GetUserAuthDTO();
            var payloadMock = AuthenticationTestData.GetPayload();

            SetupVerifyGoogleToken(payloadMock);

            var userMock = UserTestData.GetTestUserList()[0];
            SetupFindByEmailAsync(userMock.Email, userMock);

            var claimsMock = AuthenticationTestData.GetClaims();

            SetupAddLoginAsync();
            SetupSetClaimsAsync(claimsMock);
            SetupCreateToken("token");
            SetupCreateRefreshToken("refToken");

            var expectedResult = new UserAuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refToken"
            };

            var result = await _authentifiactioService.ExternalLoginAsync(userAuthDTO);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TearDown]
        public void TearDown()
        {
            _userManagerMock.Verify();
            _signInManagerMock.Verify();
            _jwtServiceMock.Verify();
            _roleManagerMock.Verify();
            _refreshTokenRepositoryMock.Verify();
            _emailSenderServiceMock.Verify();
            _confirmEmailServiceMock.Verify();
            _templateServiceMock.Verify();
            _clientUrlMock.Verify();
        }

        protected void SetupCreateRefreshToken(string token)
        {
            _jwtServiceMock
                .Setup(x => x.CreateRefreshToken())
                .Returns(token)
                .Verifiable();
        }

        protected void SetupCreateToken(string token)
        {
            _jwtServiceMock
                .Setup(x => x.CreateToken(It.IsAny<System.Collections.Generic.IEnumerable<System.Security.Claims.Claim>>()))
                .Returns(token)
                .Verifiable();
        }

        protected void SetupSetClaimsAsync(IEnumerable<Claim> claims)
        {
            _jwtServiceMock
                .Setup(x => x.SetClaims(It.IsAny<User>()))
                .Returns(claims)
                .Verifiable();
        }

        protected void SetupAddLoginAsync()
        {
            _userManagerMock
                .Setup(x => x.AddLoginAsync(It.IsAny<User>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();
        }

        protected void SetupVerifyGoogleToken(GoogleJsonWebSignature.Payload payload)
        {
            _jwtServiceMock
                .Setup(x => x.VerifyGoogleToken(It.IsAny<UserExternalAuthDTO>()))
                .Returns(Task.FromResult(payload))
                .Verifiable();
        }

        protected void SetupFindByEmailAsync(string email, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(email ?? It.IsAny<string>()))
                .Returns(Task.FromResult(userInstance))
                .Verifiable();
        }

        protected void SetupCheckPasswordAsync(User user, string password)
        {
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, password ?? It.IsAny<string>()))
                .Returns(Task.FromResult(true))
                .Verifiable();
        }
    }
}
