using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.RefreshTokenEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using System;
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
