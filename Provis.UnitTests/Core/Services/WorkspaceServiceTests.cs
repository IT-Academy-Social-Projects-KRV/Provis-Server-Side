using App.Metrics;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.WorkspaceDTO;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    class WorkspaceServiceTests
    {
        protected WorkspaceService _workspaceService;

        protected Mock<IEmailSenderService> _emailSendServiceMock;
        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IRepository<Workspace>> _workspaceRepositoryMock;
        protected Mock<IRepository<UserWorkspace>> _userWorkspaceRepositoryMock;
        protected Mock<IRepository<InviteUser>> _inviteUserRepositoryMock;
        protected Mock<IRepository<User>> _userRepositoryMock;
        protected Mock<IRepository<Role>> _userRoleRepositoryMock;
        protected Mock<IRepository<UserTask>> _userTaskRepositoryMock;
        protected Mock<IMapper> _mapperMock;
        protected Mock<RoleAccess> _roleAccessMock;
        protected Mock<ITemplateService> _templateServiceMock;
        protected Mock<IOptions<ClientUrl>> _optionsMock;
        protected Mock<IMetrics> _metricsMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _emailSendServiceMock = new Mock<IEmailSenderService>();
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _workspaceRepositoryMock = new Mock<IRepository<Workspace>>();
            _userWorkspaceRepositoryMock = new Mock<IRepository<UserWorkspace>>();
            _inviteUserRepositoryMock = new Mock<IRepository<InviteUser>>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _userRoleRepositoryMock = new Mock<IRepository<Role>>();
            _userTaskRepositoryMock = new Mock<IRepository<UserTask>>();
            _mapperMock = new Mock<IMapper>();
            _roleAccessMock = new Mock<RoleAccess>();
            _templateServiceMock = new Mock<ITemplateService>();
            _optionsMock = new Mock<IOptions<ClientUrl>>();
            _metricsMock = new Mock<IMetrics>();

            _workspaceService = new WorkspaceService(
                _userRepositoryMock.Object,
                _userManagerMock.Object,
                _workspaceRepositoryMock.Object,
                _userWorkspaceRepositoryMock.Object,
                _inviteUserRepositoryMock.Object,
                _userRoleRepositoryMock.Object,
                _emailSendServiceMock.Object,
                _userTaskRepositoryMock.Object,
                _mapperMock.Object,
                _roleAccessMock.Object,
                _templateServiceMock.Object,
                _optionsMock.Object,
                _metricsMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _userRepositoryMock.Verify();
            _userManagerMock.Verify();
            _workspaceRepositoryMock.Verify();
            _userWorkspaceRepositoryMock.Verify();
            _inviteUserRepositoryMock.Verify();
            _userRoleRepositoryMock.Verify();
            _emailSendServiceMock.Verify();
            _userTaskRepositoryMock.Verify();
            _mapperMock.Verify();
            _roleAccessMock.Verify();
            _templateServiceMock.Verify();
            _optionsMock.Verify();
            _metricsMock.Verify();
        }

        [Test]
        [TestCase("1")]
        public async Task CreateWorkspaceAsync_UserIsValidAndDTOIsValid_ReturnCompletedTask(string userid)
        {
            var workspaceCreateDTOMock = WorkspaceTestData.GetWorkspaceCreateDTO();
            var workspaceMock = WorkspaceTestData.GetTestWorkspace();

            SetupAddAsync(workspaceMock);

            await _workspaceService.CreateWorkspaceAsync(workspaceCreateDTOMock, userid);
        }

        protected void SetupAddAsync(Workspace workspaceInstance)
        {
            _workspaceRepositoryMock
                .Setup(x => x.AddAsync(workspaceInstance))
                .Verifiable();
        }

        protected void SetupUserWorkspaceAddAsync(UserWorkspace userWorkspaceInstance)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<UserWorkspace>()))
                .Verifiable();
        }
    }
}
