﻿using FluentAssertions;
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
        [TestCase("test1@gmail.com")]
        public async Task LoginAsync_UserNotExist_ThrowException(string email)
        {
            var userMock = UserTestData.GetTestUser();

            SetupFindByEmailAsync(email, null);

            Func<Task<UserAutorizationDTO>> act = () => _authentificationService.LoginAsync(email, null);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .WithMessage(ErrorMessages.IncorrectLoginOrPassword);
        }

        [Test]
        [TestCase("test1@gmail.com")]
        public async Task LoginAsync_2FaEnabledInvalidProvider_ThrowException(string email)
        {
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            IList<string> list = new List<string>() {"ssss"};

            SetupFindByEmailAsync(email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, true);
            SetupGetValidTwoFactorProviders(userMock, list);

            Func<Task<UserAutorizationDTO>> act = () => _authentificationService.LoginAsync(email, null);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .WithMessage(ErrorMessages.Invalid2StepVerification);
        }

        [Test]
        [TestCase("test1@gmail.com")]
        public async Task LoginAsync_2FaEnabled_ReturnUserAutorizationDTO(string email)
        {
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            var uriStringMock = "http://localhost:4200/";
            var viewName = "Mails/TwoFactorCode";
            var templateStringMock = "template";
            Uri uriMock = new Uri(uriStringMock);

            IList<string> list = new List<string>()
            {
                "ssss",
                "Email"
            };
            var expectedUserAutorizationDTO = new UserAutorizationDTO()
            {
                Is2StepVerificationRequired = true,
                Provider = "Email"
            };
            

            SetupFindByEmailAsync(email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, true);
            SetupGetValidTwoFactorProviders(userMock, list);
            SetupGenerateTwoFactorTokenAsync(userMock, "Email");
            SetupSendEmail();

            SetupGetTemplateHtmlAsStringAsync(viewName, templateStringMock);
            SetupApplicationUrl(new() { ApplicationUrl = uriMock });

            var result = await _authentificationService.LoginAsync(email, userPassword);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUserAutorizationDTO);
        }

        [Test]
        [TestCase("test1@gmail.com")]
        public async Task LoginAsync_2FaDisabled_ReturnUserAutorizationDTO(string email)
        {
            var claims = AuthentificationTestData.GetClaimList();
            var userMock = UserTestData.GetTestUser();
            string userPassword = "Password_1";
            var expectedUserAutorizationDTO = new UserAutorizationDTO()
            {
                Token = "token",
                RefreshToken = "refreshToken"
            };

            SetupFindByEmailAsync(email, userMock);
            SetupCheckPasswordAsync(userMock, userPassword);
            SetupGetTwoFactorEnabled(userMock, false);
            SetupSetClaims(userMock, claims);
            SetupCreateToken(claims, "token");
            SetupCreateRefreshToken("refreshToken");

            var result = await _authentificationService.LoginAsync(email, userPassword);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUserAutorizationDTO);
        }

        [Test]
        public async Task LoginTwoStepAsync_InvalidVerifiction_ThrowException()
        {
            var userMock = UserTestData.GetTestUser();
            var userTwoFactorDTOMock = AuthentificationTestData.GetUserTwoFactorDTO();

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
            var userTwoFactorDTOMock = AuthentificationTestData.GetUserTwoFactorDTO();
            var claims = AuthentificationTestData.GetClaimList();
            var expectedUserAutorizationDTO = new UserAutorizationDTO()
            {
                Token = "token",
                RefreshToken = "refreshToken"
            };

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
                .Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true))
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
                .Returns(Task.FromResult(result))
                .Verifiable();
        }

        protected void SetupGenerateTwoFactorTokenAsync(User user, string tokenProvider)
        {
            _userManagerMock
                .Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult("OK"))
                .Verifiable();
        }

        protected void SetupSendEmail()
        {
            _emailSenderServiceMock
                .Setup(x => x.SendEmailAsync(It.IsAny<MailRequest>()))
                .Returns(Task.CompletedTask)
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
    }
}
