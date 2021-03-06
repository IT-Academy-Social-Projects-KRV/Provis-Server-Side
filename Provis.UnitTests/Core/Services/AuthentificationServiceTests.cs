using Ardalis.Specification;
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
using Provis.Core.Helpers.Mails.ViewModels;
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
        protected AuthenticationService _authentificationService;

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
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _signInManagerMock = SignInManagerMock.GetSignInManager<User>();
            _roleManagerMock = RoleManagerMock.GetRoleManager<IdentityRole>();

            _jwtServiceMock = new Mock<IJwtService>();
            _refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken>>();
            _emailSenderServiceMock = new Mock<IEmailSenderService>();
            _confirmEmailServiceMock = new Mock<IConfirmEmailService>();
            _templateServiceMock = new Mock<ITemplateService>();
            _clientUrlMock = new Mock<IOptions<ClientUrl>>();

            _authentificationService = new AuthenticationService(
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

        [Test]
        public async Task LoginAsync_UserNotExist_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();

            SetupFindByEmailAsync(userMock.Email, null);

            Func<Task<UserAutorizationDTO>> act = () => 
                _authentificationService.LoginAsync(userMock.Email, null);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .WithMessage(ErrorMessages.IncorrectLoginOrPassword);
        }

        [Test]
        public async Task LoginAsync_2FaEnabledInvalidProvider_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            IList<string> list = new List<string>() { "Invalid provider" };

            SetupFindByEmailAsync(userMock.Email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, true);
            SetupGetValidTwoFactorProviders(userMock, list);

            Func<Task<UserAutorizationDTO>> act = () => 
                _authentificationService.LoginAsync(userMock.Email, null);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .WithMessage(ErrorMessages.Invalid2StepVerification);
        }

        [Test]
        public async Task LoginAsync_2FaEnabled_ReturnUserAutorizationDTO()
        {
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            var uriStringMock = "http://localhost:4200/";
            var viewName = "Mails/TwoFactorCode";
            var templateStringMock = "template";
            Uri uriMock = new Uri(uriStringMock);

            IList<string> list = new List<string>()
            {
                "Provider",
                "Email"
            };
            var expectedUserAutorizationDTO = new UserAutorizationDTO()
            {
                Is2StepVerificationRequired = true,
                Provider = "Email"
            };

            SetupFindByEmailAsync(userMock.Email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, true);
            SetupGetValidTwoFactorProviders(userMock, list);
            SetupGenerateTwoFactorTokenAsync(userMock, "Email");
            SetupSendEmail();
            SetupGetTemplateHtmlAsStringAsync(viewName, templateStringMock);
            SetupApplicationUrl(new() { ApplicationUrl = uriMock });

            var result = await _authentificationService.LoginAsync(userMock.Email, userPassword);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUserAutorizationDTO);
        }

        [Test]
        public async Task LoginAsync_2FaDisabled_ReturnUserAutorizationDTO()
        {
            var claims = GetClaimList();
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            var expectedUserAutorizationDTO = GetUserAutorizationDTO();

            SetupFindByEmailAsync(userMock.Email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, false);
            SetupSetClaims(userMock, claims);
            SetupCreateToken(claims, "token");
            SetupCreateRefreshToken("refreshToken");

            var result = await _authentificationService.LoginAsync(userMock.Email, userPassword);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUserAutorizationDTO);
        }

        [Test]
        public async Task LoginTwoStepAsync_InvalidVerifiction_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();
            var userTwoFactorDTOMock = GetUserTwoFactorDTO();

            SetupFindByEmailAsync(userMock.Email, userMock);
            SetupVerifyTwoFactorTokenAsync(userMock,
                userTwoFactorDTOMock.Provider,
                userTwoFactorDTOMock.Token,
                false);

            Func<Task<UserAutorizationDTO>> act = () =>
                _authentificationService.LoginTwoStepAsync(userTwoFactorDTOMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InvalidTokenVerification);
        }

        [Test]
        public async Task LoginTwoStepAsync_ValidVerifiction_ReturnUserAutorizationDTO()
        {
            var userMock = UserTestData.GetTestUser();
            var userTwoFactorDTOMock = GetUserTwoFactorDTO();
            var claims = GetClaimList();
            var expectedUserAutorizationDTO = GetUserAutorizationDTO();

            SetupFindByEmailAsync(userMock.Email, userMock);
            SetupSetClaims(userMock, claims);
            SetupCreateToken(claims, "token");
            SetupCreateRefreshToken("refreshToken");
            SetupVerifyTwoFactorTokenAsync(userMock,
                userTwoFactorDTOMock.Provider,
                userTwoFactorDTOMock.Token,
                true);

            var result = await _authentificationService.LoginTwoStepAsync(userTwoFactorDTOMock);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUserAutorizationDTO);
        }

        [Test]
        public async Task RegistrationAsync_CreateResultFalse_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();
            string password = "password";
            string roleName = "role";

            SetupCreateAsync(userMock, 
                password, 
                IdentityResult.Failed(new IdentityError 
                    { Description = "An unknown failure has occurred." }));

            Func<Task> act = () =>
                _authentificationService.RegistrationAsync(userMock,
                password,
                roleName);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage("An unknown failure has occurred. ");
        }

        [Test]
        public async Task RegistrationAsync_RoleNull_ReturnTaskCompleted()
        {
            var userMock = UserTestData.GetTestUser();
            string password = "password";
            string roleName = "role";

            SetupCreateAsync(userMock,
                password,
                IdentityResult.Success);
            SetupFindByNameAsync(roleName, null);
            SetupCreateAsync(It.IsAny<IdentityRole>(), IdentityResult.Success);
            SetupAddToRoleAsync(userMock, roleName, IdentityResult.Success);

            var result = _authentificationService.RegistrationAsync(userMock, password, roleName);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task RefreshTokenAsync_RefreshTokenNull_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();
            var userAutorizationDTO = GetUserAutorizationDTO();
            RefreshToken refreshToken = null;

            SetupGetFirstBySpecAsync(refreshToken);
            Func<Task<UserAutorizationDTO>> act = () =>
                _authentificationService.RefreshTokenAsync(userAutorizationDTO);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InvalidToken);
        }

        [Test]
        public async Task RefreshTokenAsync_RefreshTokenValid_ReturnUserAutorizationDTO()
        {
            var userMock = UserTestData.GetTestUser();
            var userAutorizationDTO = GetUserAutorizationDTO();
            var claims = GetClaimList();
            string token = "token";
            var expiredRefreshToken = new RefreshToken();
            RefreshToken refreshToken = new RefreshToken()
            {
                Id = 1,
                UserId = userMock.Id,
                User = userMock,
                Token = "refreshToken"
            };

            SetupGetFirstBySpecAsync(expiredRefreshToken);
            SetupGetClaimsFromExpiredToken("token", claims);
            SetupCreateToken(claims, token);
            SetupCreateRefreshToken(refreshToken.Token);
            SetupUpdateAsync(expiredRefreshToken);
            SetupSaveChangesAsync();

            var result = await _authentificationService.RefreshTokenAsync(userAutorizationDTO);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(userAutorizationDTO);
        }

        [Test]
        public void LogoutAsync_RefreshTokenNull_RefreshTokenIsNull()
        {
            var userAutorizationDTO = GetUserAutorizationDTO();

            SetupGetFirstBySpecAsync(null);

            var result = Task.Run(async () =>
                await _authentificationService.LogoutAsync(userAutorizationDTO));
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Test]
        public void LogoutAsync_RefreshTokenNotNull_RefreshTokenDeleted()
        {
            var userMock = UserTestData.GetTestUser();
            var userAutorizationDTO = GetUserAutorizationDTO();
            var refreshToken = GetRefreshToken();

            SetupGetFirstBySpecAsync(refreshToken);
            SetupDeleteAsync(refreshToken);
            SetupSaveChangesAsync();

            var result = Task.Run(() =>
                _authentificationService.LogoutAsync(userAutorizationDTO));
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Test]
        public async Task SentResetPasswordTokenAsync_UserNull_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();

            SetupFindByEmailAsync(userMock.Email, null);
            Func<Task> act = () =>
                _authentificationService.SentResetPasswordTokenAsync(userMock.Email);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.UserNotFound);
        }

        [Test]
        public async Task ResetPasswordAsync_IdentityResultFalse_ReturnTaskCompleted()
        {
            UserChangePasswordDTO userChangePasswordDTO = GetUserChangePasswordDTO();
            var userMock = UserTestData.GetTestUser();
            string decodedCode = "decodedCode";

            SetupFindByEmailAsync(userMock.Email, 
                userMock);
            SetupDecodeUnicodeBase64(userChangePasswordDTO.Code, 
                decodedCode);
            SetupResetPasswordAsync(userMock, decodedCode, 
                userChangePasswordDTO.NewPassword, 
                IdentityResult.Success);

            var result = _authentificationService.ResetPasswordAsync(userChangePasswordDTO);
            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task ResetPasswordAsync_IdentityResultFalse_ThrowException()
        {
            UserChangePasswordDTO userChangePasswordDTO = GetUserChangePasswordDTO();
            var userMock = UserTestData.GetTestUser();
            string decodedCode = "decodedCode";

            SetupFindByEmailAsync(userMock.Email,
                userMock);
            SetupDecodeUnicodeBase64(userChangePasswordDTO.Code,
                decodedCode);
            SetupResetPasswordAsync(userMock, decodedCode,
                userChangePasswordDTO.NewPassword,
                IdentityResult.Failed());

            Func<Task> act = () =>
                _authentificationService.ResetPasswordAsync(userChangePasswordDTO);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.WrongResetPasswordCode);

            await Task.CompletedTask;
        }

        [Test]
        public async Task ExternalLoginAsync_PayloadIsInvalid_ThrowHttpException()
        {
            var authDTOMock = GetUserAuthDTO();

            Func<Task> act = () =>
                _authentificationService.ExternalLoginAsync(authDTOMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InvalidRequest);
        }

        [Test]
        public async Task ExternalLoginAsync_PayloadIsValid_ReturnUserAuthResponseDTO()
        {
            var userAuthDTO = GetUserAuthDTO();
            var payloadMock = GetPayload();

            SetupVerifyGoogleToken(payloadMock);

            var userMock = GetTestUserList()[0];
            SetupFindByEmailAsync(userMock.Email, userMock);

            var claimsMock = GetClaims();

            SetupAddLoginAsync();
            SetupSetClaimsAsync(claimsMock);
            SetupCreateToken("token");
            SetupCreateRefreshToken("refToken");

            var expectedResult = new UserAuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refToken"
            };

            var result = await _authentificationService.ExternalLoginAsync(userAuthDTO);

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

        protected void SetupCreateToken(string token)
        {
            _jwtServiceMock
                .Setup(x => x.CreateToken(It.IsAny<IEnumerable<Claim>>()))
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
                .ReturnsAsync(payload)
                .Verifiable();
        }

        protected void SetupFindByEmailAsync(string email, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(email ?? It.IsAny<string>()))
                .ReturnsAsync(userInstance)
                .Verifiable();
        }

        protected void SetupCheckPasswordAsync(User user, string password)
        {
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();
        }

        protected void SetupGetValidTwoFactorProviders(User user, IList<string> list)
        {
            _userManagerMock
                .Setup(x => x.GetValidTwoFactorProvidersAsync(user))
                .ReturnsAsync(list)
                .Verifiable();
        }

        protected void SetupGetTwoFactorEnabled(User user, bool result)
        {
            _userManagerMock
                .Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<User>()))
                .ReturnsAsync(result)
                .Verifiable();
        }

        protected void SetupGenerateTwoFactorTokenAsync(User user, string tokenProvider)
        {
            _userManagerMock
                .Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync("OK")
                .Verifiable();
        }

        protected void SetupSendEmail()
        {
            _emailSenderServiceMock
                .Setup(x => x.SendEmailAsync(It.IsAny<MailRequest>()))
                .Verifiable();
        }

        protected void SetupGetTemplateHtmlAsStringAsync(string viewName,
            string templateInstance)
        {
            _templateServiceMock
                .Setup(x => x.GetTemplateHtmlAsStringAsync(viewName ?? It.IsAny<string>(),
                    It.IsAny<UserToken>()))
                .ReturnsAsync(templateInstance)
                .Verifiable();
        }
        protected void SetupApplicationUrl(ClientUrl clientUri)
        {
            _clientUrlMock
                .Setup(x => x.Value)
                .Returns(clientUri)
                .Verifiable();
        }

        protected void SetupSetClaims(User user, IEnumerable<Claim> claims)
        {
            _jwtServiceMock
                .Setup(x => x.SetClaims(It.IsAny<User>()))
                .Returns(claims)
                .Verifiable();
        }

        protected void SetupCreateToken(IEnumerable<Claim> claims, string token)
        {
            _jwtServiceMock
                .Setup(x => x.CreateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(token)
                .Verifiable();
        }

        protected void SetupCreateRefreshToken(string refreshToken)
        {
            _jwtServiceMock
               .Setup(x => x.CreateRefreshToken())
               .Returns(refreshToken)
               .Verifiable();
        }

        protected void SetupVerifyTwoFactorTokenAsync(User user,
            string tokenProvider,
            string token,
            bool result)
        {
            _userManagerMock
               .Setup(x => x.VerifyTwoFactorTokenAsync(user ?? It.IsAny<User>(),
                    tokenProvider ?? It.IsAny<string>(),
                    token ?? It.IsAny<string>()))
               .ReturnsAsync(result)
               .Verifiable();
        }

        protected void SetupGetFirstBySpecAsync(RefreshToken refreshToken)
        {
            _refreshTokenRepositoryMock
                .Setup(x => x.GetFirstBySpecAsync(It.IsAny<ISpecification<RefreshToken>>()))
                .ReturnsAsync(refreshToken)
                .Verifiable();
        }


        protected void SetupGetClaimsFromExpiredToken(string token, IEnumerable<Claim> claims)
        {
            _jwtServiceMock
                .Setup(x => x.GetClaimsFromExpiredToken(token ?? It.IsAny<string>()))
                .Returns(claims)
                .Verifiable();
        }

        protected void SetupSaveChangesAsync()
        {
            _refreshTokenRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupUpdateAsync(RefreshToken refreshToken)
        {
            _refreshTokenRepositoryMock
                .Setup(x => x.UpdateAsync(refreshToken))
                .Verifiable();
        }

        protected void SetupDeleteAsync(RefreshToken refreshToken)
        {
            _refreshTokenRepositoryMock
                .Setup(x => x.DeleteAsync(refreshToken))
                .Verifiable();
        }

        protected void SetupGeneratePasswordResetTokenAsync(User user, string token)
        {
            _userManagerMock
                .Setup(x => x.GeneratePasswordResetTokenAsync(user ?? It.IsAny<User>()))
                .ReturnsAsync(token)
                .Verifiable();
        }

        protected void SetupDecodeUnicodeBase64(string code, string result)
        {
            _confirmEmailServiceMock
                .Setup(x => x.DecodeUnicodeBase64(code ?? It.IsAny<string>()))
                .Returns(result)
                .Verifiable();
        }

        protected void SetupResetPasswordAsync(User user,
            string decodedCode,
            string newPassword,
            IdentityResult result)
        {
            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(user, decodedCode, newPassword))
                .ReturnsAsync(result)
                .Verifiable();
        }

        protected void SetupCreateAsync(User user, string password, IdentityResult result)
        {
            _userManagerMock
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(result)
                .Verifiable();
        }

        protected void SetupFindByNameAsync(string roleName, IdentityRole role)
        {
            _roleManagerMock
                .Setup(x => x.FindByNameAsync(roleName))
                .ReturnsAsync(role)
                .Verifiable();
        }

        protected void SetupCreateAsync(IdentityRole role, IdentityResult result)
        {
            _roleManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(result)
                .Verifiable();
        }

        protected void SetupAddToRoleAsync(User user, string role, IdentityResult result)
        {
            _userManagerMock
                .Setup(x => x.AddToRoleAsync(user, role))
                .ReturnsAsync(result)
                .Verifiable();
        }

        public UserExternalAuthDTO GetUserAuthDTO()
        {
            return new UserExternalAuthDTO()
            {
                IdToken = "1",
                Provider = "2"
            };
        }

        public GoogleJsonWebSignature.Payload GetPayload()
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

        public IEnumerable<Claim> GetClaims()
        {
            return new List<Claim>
            {
                new Claim("1","2"),
                new Claim("3","4")
            };
        }

        public List<User> GetTestUserList()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = "1",
                    Email = "test1@gmail.com",
                    Name = "Name1",
                    Surname = "Surname1",
                    UserName = "Username1",
                    ImageAvatarUrl = "Path1"
                },

                new User()
                {
                    Id = "2",
                    Email = "test2@gmail.com",
                    Name = "Name2",
                    Surname = "Surname2",
                    UserName = "Username2",
                    ImageAvatarUrl = "Path2"
                }
            };
        }

        private RefreshToken GetRefreshToken()
        {
            return new RefreshToken()
            {
                Id = 1,
                UserId = "1",
                User = new User(),
                Token = "refreshToken"
            };
        }

        protected UserAutorizationDTO GetUserAutorizationDTO()
        {
            return new UserAutorizationDTO()
            {
                Token = "token",
                RefreshToken = "refreshToken"
            };
        }

        private UserChangePasswordDTO GetUserChangePasswordDTO()
        {
            return new UserChangePasswordDTO()
            {
                Email = "test1@gmail.com",
                Code = "Code",
                NewPassword = "password"
            };
        }

        public List<Claim> GetClaimList()
        {
            return new List<Claim>()
            {
                new Claim("type", "value") {}
            };
        }

        public UserTwoFactorDTO GetUserTwoFactorDTO()
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
