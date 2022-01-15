using AutoMapper;
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
using System.Net;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _userService;

        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IRepository<User>> _userRepositoryMock;
        private Mock<IRepository<InviteUser>> _inviteUserRepositoryMock;
        private Mock<IEmailSenderService> _emailSenderServiceMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IOptions<ImageSettings>> _imageSettingsMock;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userManagerMock = MockUserManager.GetUserManager<User>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _inviteUserRepositoryMock = new Mock<IRepository<InviteUser>>();
            _emailSenderServiceMock = new Mock<IEmailSenderService>();
            _fileServiceMock = new Mock<IFileService>();
            _imageSettingsMock = new Mock<IOptions<ImageSettings>>();

            _mapper = MapperForTests.GetMapper();

            _userService = new UserService(
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _inviteUserRepositoryMock.Object,
                _mapper,
                _emailSenderServiceMock.Object,
                _fileServiceMock.Object,
                _imageSettingsMock.Object);
        }

        [Test]
        public async Task GetUserPersonalInfoAsync_UserExist_ReturnUserPersonalInfoDTO()
        {
            var userId = "1";
            var userMock = TestData.GetTestUser();

            _userRepositoryMock
                .Setup(x => x.GetByKeyAsync(userId))
                .ReturnsAsync(userMock);

            var expectedUser = _mapper.Map<UserPersonalInfoDTO>(userMock);

            var result = await _userService.GetUserPersonalInfoAsync(userId);

            Assert.NotNull(result);
            Assert.AreEqual(expectedUser.Email, result.Email);
        }

        [Test]
        public Task GetUserPersonalInfoAsync_UserNotExist_ThrowHttpException()
        {
            var userMock = TestData.GetTestUser();

            _userRepositoryMock
                .Setup(x => x.GetByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<User>(null));

            var exeption = Assert.ThrowsAsync<HttpException>(() => _userService.GetUserPersonalInfoAsync("1"));

            Assert.NotNull(exeption);
            Assert.AreEqual(exeption.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(exeption.Message, "User with Id not exist");

            return Task.CompletedTask;
        }
    }
}
