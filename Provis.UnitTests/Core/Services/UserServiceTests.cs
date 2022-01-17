using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using System;
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

            _userService = new UserService(
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _inviteUserRepositoryMock.Object,
                _mapperMock.Object,
                _emailSenderServiceMock.Object,
                _fileServiceMock.Object,
                _imageSettingsMock.Object);
        }

        [Test]
        [TestCase("1")]
        public async Task GetUserPersonalInfoAsync_UserExist_ReturnUserPersonalInfoDTO(string userId)
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
        public async Task GetUserPersonalInfoAsync_UserNotExist_ThrowHttpException()
        {
            var userMock = UserTestData.GetTestUser();

            SetupUserGetByKeyAsync(null, null);

            Func<Task> act = () =>
                _userService.GetUserPersonalInfoAsync(It.IsAny<string>());

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x=>x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage("User with Id not exist");
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
        /// Mocks UserManager.GetByKeyAsync
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

        //TODO Local data should be configured here
        //IT can be virtual
    }
}
