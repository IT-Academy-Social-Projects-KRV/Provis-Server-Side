using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.ApiModels;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers;
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
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        protected UserService _userService;

        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IRepository<User>> _userRepositoryMock;
        protected Mock<IRepository<InviteUser>> _inviteUserRepositoryMock;
        protected Mock<IMapper> _mapperMock;
        protected Mock<IEmailSenderService> _emailSenderServiceMock;
        protected Mock<IFileService> _fileServiceMock;
        protected Mock<IOptions<ImageSettings>> _imageSettingsMock;
        protected Mock<ITemplateService> _templateServiceMock;
        protected Mock<IOptions<ClientUrl>> _clientUrlMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _inviteUserRepositoryMock = new Mock<IRepository<InviteUser>>();
            _mapperMock = new Mock<IMapper>();
            _emailSenderServiceMock = new Mock<IEmailSenderService>();
            _fileServiceMock = new Mock<IFileService>();
            _imageSettingsMock = new Mock<IOptions<ImageSettings>>();
            _templateServiceMock = new Mock<ITemplateService>();
            _clientUrlMock = new Mock<IOptions<ClientUrl>>();

            _userService = new UserService(
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _inviteUserRepositoryMock.Object,
                _mapperMock.Object,
                _emailSenderServiceMock.Object,
                _fileServiceMock.Object,
                _imageSettingsMock.Object,
                _templateServiceMock.Object,
                _clientUrlMock.Object);
        }

        [Test]
        [TestCase("1")]
        public async Task GetUserPersonalInfoAsync_UserExist_ReturnUserPersonalInfo(string userId)
        {
            var userMock = UserTestData.GetTestUser();
            var expectedUser = new UserPersonalInfoDTO()
            {
                Name = userMock.Name,
                Surname = userMock.Surname,
                Email = userMock.Email,
                Username = userMock.UserName
            };

            SetupUserGetByKeyAsync(userId, userMock);
            _mapperMock.SetupMap(userMock, expectedUser);

            var result = await _userService.GetUserPersonalInfoAsync(userId);

            result.Should().NotBeNull();
            result.Should().Be(expectedUser);
        }

        [Test]
        public async Task ChangeInfoAsync_UserChandeInfoIsValid_ReturnTaskComple()
        {
            var userMock = UserTestData.GetTestUser();
            var userChangeInfoMock = GetUserChangeInfo();
            var userId = userMock.Id;

            SetupUserFindByNameAsync(userChangeInfoMock.UserName, null);
            SetupUserGetByKeyAsync(userId, userMock);
            SetupMap(userChangeInfoMock, userMock);
            SetupUserRepositoryUpdateAsync();
            SetupUpdateNormalizedUserNameAsync();
            SetupSaveChangesAsync();

            var result = _userService.ChangeInfoAsync(userId, userChangeInfoMock);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task ChangeInfoAsync_UsernameAlredyTaken_ThrowHttpException()
        {
            var userMock = UserTestData.GetTestUser();
            var userChangeInfoMock = GetUserChangeInfo();
            var userId = "2";

            userMock.UserName = userChangeInfoMock.UserName;

            SetupUserFindByNameAsync(userChangeInfoMock.UserName, userMock);

            Func<Task> act = () =>
                _userService.ChangeInfoAsync(userId, userChangeInfoMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UsernameAlreadyExists);
        }

        [Test]
        [TestCase("1")]
        public async Task GetUserInviteInfoListAsync_UserIdIsValid_ReturnInviteList(string userId)
        {
            var inviteUserListMock = InviteUserTestData.GetInviteUserList();
            var expectedInviteUserListInfo = GetUserInviteInfoList();

            SetupGetListBySpecAsync(inviteUserListMock);
            _mapperMock.SetupMap(inviteUserListMock, expectedInviteUserListInfo);

            var result = await _userService.GetUserInviteInfoListAsync(userId);

            result.Should().NotBeNullOrEmpty();
            result.Should().Equal(expectedInviteUserListInfo);
        }

        [Test]
        [TestCase("1")]
        public async Task IsActiveInviteAsync_UserIdIsValid_ReturnUserActiveInvite(string userId)
        {
            var userActiveInviteExpected = GetUserActiveInvite();

            SetupAnyBySpecAsync(userActiveInviteExpected.IsActiveInvite);

            var result = await _userService.IsActiveInviteAsync(userId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(userActiveInviteExpected);
        }

        [Test]
        public async Task UpdateUserImageAsync_UserNotHaveImage_ReturnTaskCompleted()
        {
            var userMock = UserTestData.GetTestUser();
            userMock.ImageAvatarUrl = null;
            var userWithAvatarMock = UserTestData.GetTestUser();

            string folderPath = "images";

            var imageMock = FileTestData.GetTestFormFile("filename", "content");

            SetupAddFileAsync(folderPath,
                imageMock.FileName,
                userWithAvatarMock.ImageAvatarUrl);

            SetupUserFindByIdAsync(userMock.Id, userMock);
            SetupImageOptions(folderPath);
            SetupUserManagerUpdateAsync();

            var result = _userService.UpdateUserImageAsync(imageMock, userMock.Id);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task UpdateUserImageAsync_UserHaveImage_ReturnTaskCompleted()
        {
            var userMock = UserTestData.GetTestUser();
            var userWithNewAvatarMock = UserTestData.GetTestUser();
            userWithNewAvatarMock.ImageAvatarUrl = "new path";

            string folderPath = "images";

            var imageMock = FileTestData.GetTestFormFile("filename", "content");

            SetupAddFileAsync(folderPath,
                imageMock.FileName,
                userWithNewAvatarMock.ImageAvatarUrl);

            SetupUserFindByIdAsync(userMock.Id, userMock);
            SetupDeleteFileAsync(userMock.ImageAvatarUrl);
            SetupImageOptions(folderPath);
            SetupUserManagerUpdateAsync();

            var result = _userService.UpdateUserImageAsync(imageMock, userMock.Id);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task GetUserImageAsync_UserHaveImage_ReturnDownloadFile()
        {
            var userMock = UserTestData.GetTestUser();
            var userId = userMock.Id;
            var downloadFileExpected = FileTestData
                .GetTestDownloadFile("name", "content type", "content");

            SetupUserFindByIdAsync(userId, userMock);
            SetupGetFileAsync(userMock.ImageAvatarUrl, downloadFileExpected);

            var result = await _userService.GetUserImageAsync(userId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(downloadFileExpected);
        }

        [Test]
        public async Task GetUserImageAsync_UserNotHaveImage_ReturnDownloadFile()
        {
            var userMock = UserTestData.GetTestUser();
            userMock.ImageAvatarUrl = null;
            var userId = userMock.Id;

            SetupUserFindByIdAsync(userId, userMock);

            Func<Task<DownloadFile>> act = () => _userService.GetUserImageAsync(userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.ImageNotFound);
        }

        [Test]
        public async Task CheckIsTwoFactorVerificationAsync_EmailIsConfirmed_ReturnBoolValue()
        {
            var usesMock = UserTestData.GetTestUser();
            usesMock.EmailConfirmed = true;
            var userId = usesMock.Id;
            var isTwoFactorEnabledExpexted = true;

            SetupUserFindByIdAsync(userId, usesMock);
            SetupGetTwoFactorEnabledAsync(usesMock, isTwoFactorEnabledExpexted);

            var result = await _userService.CheckIsTwoFactorVerificationAsync(userId);

            result.Should().Be(isTwoFactorEnabledExpexted);
        }

        [Test]
        public async Task CheckIsTwoFactorVerificationAsync_EmailIsNotConfirmed_ThrowHttpException()
        {
            var usesMock = UserTestData.GetTestUser();
            var userId = usesMock.Id;

            SetupUserFindByIdAsync(userId, usesMock);

            Func<Task<bool>> act = () => _userService.CheckIsTwoFactorVerificationAsync(userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.EmailNotConfirm);
        }

        [Test]
        public async Task ChangeTwoFactorVerificationStatusAsync_UserTokenValid_ReturnTaskCompleted()
        {
            var userMock = UserTestData.GetTestUser();
            var userChange2faStatusMock = GetUserChange2faStatus();
            var userId = userMock.Id;

            SetupUserFindByIdAsync(userId, userMock);
            SetupVerifyTwoFactorTokenAsync(userMock,
                "Email",
                userChange2faStatusMock.Token,
                true);
            SetupGetTwoFactorEnabledAsync(userMock, true);
            SetupSetTwoFactorEnabledAsync(userMock, false, IdentityResult.Success);

            var result = _userService
                .ChangeTwoFactorVerificationStatusAsync(userId, userChange2faStatusMock);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task ChangeTwoFactorVerificationStatusAsync_UserTokenInValid_ThrowHttpException()
        {
            var userMock = UserTestData.GetTestUser();
            var userChange2faStatusMock = GetUserChange2faStatus();
            var userId = userMock.Id;

            SetupUserFindByIdAsync(userId, userMock);
            SetupVerifyTwoFactorTokenAsync(userMock,
                "Email",
                userChange2faStatusMock.Token,
                false);

            Func<Task> act = () => _userService
                .ChangeTwoFactorVerificationStatusAsync(userId, userChange2faStatusMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.Invalid2FVCode);
        }

        [Test]
        public async Task ChangeTwoFactorVerificationStatusAsync_IdentityResultIsFailed_ThrowHttpException()
        {
            var userMock = UserTestData.GetTestUser();
            var userChange2faStatusMock = GetUserChange2faStatus();
            var userId = userMock.Id;

            SetupUserFindByIdAsync(userId, userMock);
            SetupVerifyTwoFactorTokenAsync(userMock,
                "Email",
                userChange2faStatusMock.Token,
                true);
            SetupGetTwoFactorEnabledAsync(userMock, true);
            SetupSetTwoFactorEnabledAsync(userMock, false, new IdentityResult());

            Func<Task> act = () => _userService
                .ChangeTwoFactorVerificationStatusAsync(userId, userChange2faStatusMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InvalidRequest);
        }

        [Test]
        public async Task SendTwoFactorCodeAsync_UserExist_ReturnTaskComplected()
        {
            var userMock = UserTestData.GetTestUser();
            var userId = userMock.Id;
            var templateStringMock = "template";
            var twoFactorTokenMock = "token";
            var uriStringMock = "http://localhost:4200/";
            var viewName = "Mails/TwoFactorCode";
            Uri uriMock = new Uri(uriStringMock);

            SetupUserFindByIdAsync(userId, userMock);
            SetupGenerateTwoFactorTokenAsync(userMock,"Email", twoFactorTokenMock);
            SetupGetTemplateHtmlAsStringAsync(viewName, templateStringMock);
            SetupApplicationUrl(new() { ApplicationUrl = uriMock });
            SetupSendEmailAsync();

            var result = _userService.SendTwoFactorCodeAsync(userId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [TearDown]
        public void TearDown()
        {
            _userManagerMock.Verify();
            _userRepositoryMock.Verify();
            _inviteUserRepositoryMock.Verify();
            _mapperMock.Verify();
            _emailSenderServiceMock.Verify();
            _fileServiceMock.Verify();
            _imageSettingsMock.Verify();
        }

        /// <summary>
        /// Mocks UserRepository.GetByKeyAsync
        /// </summary>
        /// <param name="key">If value null it is going to use It.
        /// IsAny<string>() For this parameter in mock setup</param>
        /// <param name="userInstance">Object of user that should be returned</param>
        protected void SetupUserGetByKeyAsync(string key, User userInstance)
        {
            _userRepositoryMock
                .Setup(x => x.GetByKeyAsync(key ?? It.IsAny<string>()))
                .Returns(Task.FromResult(userInstance))
                .Verifiable();
        }

        /// <summary>
        /// Mocks UserManager.FindByNameAsync
        /// </summary>
        /// <param name="username">If value null it is going to use It.
        /// IsAny<string>() For this parameter in mock setup</param>
        /// <param name="userInstance">Object of user that should be returned</param>
        protected void SetupUserFindByNameAsync(string username, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByNameAsync(username ?? It.IsAny<string>()))
                .ReturnsAsync(userInstance)
                .Verifiable();
        }

        protected void SetupUserFindByIdAsync(string userId, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId ?? It.IsAny<string>()))
                .ReturnsAsync(userInstance)
                .Verifiable();
        }

        protected void SetupUserRepositoryUpdateAsync()
        {
            _userRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupUserManagerUpdateAsync()
        {
            _userManagerMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(new IdentityResult())
                .Verifiable();
        }

        protected void SetupUpdateNormalizedUserNameAsync()
        {
            _userManagerMock
                .Setup(x => x.UpdateNormalizedUserNameAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupSaveChangesAsync()
        {
            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();
        }

        protected void SetupGetListBySpecAsync(List<InviteUser> listInstance)
        {
            _inviteUserRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<InviteUser>>()))
                .ReturnsAsync(listInstance)
                .Verifiable();
        }

        protected void SetupMap<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapperMock
                .Setup(x => x.Map(source ?? It.IsAny<TSource>(), destination))
                .Returns(destination)
                .Verifiable();
        }

        protected void SetupAnyBySpecAsync(bool boolInstance)
        {
            _inviteUserRepositoryMock
                .Setup(x => x.AnyBySpecAsync(It.IsAny<ISpecification<InviteUser>>()))
                .ReturnsAsync(boolInstance)
                .Verifiable();
        }

        protected void SetupAddFileAsync(string folderPath,
            string fileName,
            string dbPathInstance)
        {
            _fileServiceMock
                .Setup(x => x.AddFileAsync(It.IsAny<Stream>(),
                                           folderPath ?? It.IsAny<string>(),
                                           fileName ?? It.IsAny<string>()))
                .Returns(Task.FromResult(dbPathInstance))
                .Verifiable();
        }

        protected void SetupGetFileAsync(string path, DownloadFile downloadFileInstance)
        {
            _fileServiceMock
                .Setup(x => x.GetFileAsync(path ?? It.IsAny<string>()))
                .ReturnsAsync(downloadFileInstance)
                .Verifiable();
        }

        protected void SetupDeleteFileAsync(string path)
        {
            _fileServiceMock
                .Setup(x => x.DeleteFileAsync(path ?? It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupImageOptions(string path)
        {
            _imageSettingsMock
                .Setup(x => x.Value)
                .Returns(new ImageSettings() { Path = path})
                .Verifiable();
        }

        protected void SetupGetTwoFactorEnabledAsync(User user, bool isTwoFactorEnabledInstance)
        {
            _userManagerMock
                .Setup(x => x.GetTwoFactorEnabledAsync(user ?? It.IsAny<User>()))
                .ReturnsAsync(isTwoFactorEnabledInstance)
                .Verifiable();
        }

        protected void SetupVerifyTwoFactorTokenAsync(User user,
            string tokenProvider,
            string token,
            bool isTokenValidInstance)
        {
            _userManagerMock
                .Setup(x => x.VerifyTwoFactorTokenAsync(user ?? It.IsAny<User>(),
                                                        tokenProvider ?? It.IsAny<string>(),
                                                        token ?? It.IsAny<string>()))
                .ReturnsAsync(isTokenValidInstance)
                .Verifiable();
        }

        protected void SetupGenerateTwoFactorTokenAsync(User user,
            string tokenProvider,
            string tokenInstance)
        {
            _userManagerMock
                .Setup(x => x.GenerateTwoFactorTokenAsync(user ?? It.IsAny<User>(),
                                                        tokenProvider ?? It.IsAny<string>()))
                .ReturnsAsync(tokenInstance)
                .Verifiable();
        }

        protected void SetupSendEmailAsync()
        {
            _emailSenderServiceMock
                .Setup(x=>x.SendEmailAsync(It.IsAny<MailRequest>()))
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

        protected void SetupSetTwoFactorEnabledAsync(User user,
            bool? enabled,
            IdentityResult identityResultInstance)
        {
            _userManagerMock
                .Setup(x => x.SetTwoFactorEnabledAsync(user ?? It.IsAny<User>(),
                                                       enabled ?? It.IsAny<bool>()))
                .ReturnsAsync(identityResultInstance);
        }

        protected UserChangeInfoDTO GetUserChangeInfo()
        {
            return new UserChangeInfoDTO()
            {
                Name = "Change name",
                Surname = "Change sarname",
                UserName = "Change username"
            };
        }

        protected List<UserInviteInfoDTO> GetUserInviteInfoList()
        {
            return new List<UserInviteInfoDTO>()
            {
                new UserInviteInfoDTO()
                {
                    Id = 1,
                    FromUserName = "username2",
                    ToUserId = "1",
                    IsConfirm = true,
                    WorkspaceName = "worksace1"
                },
                new UserInviteInfoDTO()
                {
                    Id = 1,
                    FromUserName = "username2",
                    ToUserId = "1",
                    IsConfirm = false,
                    WorkspaceName = "worksace2"
                },
                new UserInviteInfoDTO()
                {
                    Id = 1,
                    FromUserName = "username2",
                    ToUserId = "1",
                    IsConfirm = true,
                    WorkspaceName = "worksace2"
                }
            };
        }

        protected UserActiveInviteDTO GetUserActiveInvite()
        {
            return new()
            {
                IsActiveInvite = true
            };
        }

        protected UserChange2faStatusDTO GetUserChange2faStatus()
        {
            return new UserChange2faStatusDTO()
            {
                Token= "token"
            };
        }
    }
}
