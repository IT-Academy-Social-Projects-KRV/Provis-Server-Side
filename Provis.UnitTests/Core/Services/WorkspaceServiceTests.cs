using App.Metrics;
using AutoMapper;
using FluentAssertions;
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
using Provis.Core.Exeptions;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
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

            SetupAddWorkspaceAsync(workspaceMock);
            SetupWorkspaceSaveChangesAsync();

            var userWorkspaceMock = WorkspaceTestData.GetTestUserWorkspace();

            SetupAddUserWorkspaceAsync(userWorkspaceMock);
            SetupUserWorkspaceSaveChangesAsync();

            var result = _workspaceService.CreateWorkspaceAsync(workspaceCreateDTOMock, userid);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task UpdateWorkspaceAsync_DTOIsValid_ReturnCompletedTask()
        {
            var workspaceCreateDTOMock = WorkspaceTestData.GetTestUpdateWorkspaceDTO();
            var workspaceMock = WorkspaceTestData.GetTestWorkspace();

            SetupGetWorkspaceByKeyAsync(workspaceCreateDTOMock.WorkspaceId, workspaceMock);
            SetupMap(workspaceCreateDTOMock, workspaceMock);

            SetupUpdateWorkspaceAsync();
            SetupWorkspaceSaveChangesAsync();

            var result = _workspaceService.UpdateWorkspaceAsync(workspaceCreateDTOMock);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase("1")]
        public async Task SendInviteAsync_WrongEmail_ThrowHTTPException(string ownerId)
        {
            var workspaceInviteDTOMock = WorkspaceTestData.GetTestWorkspaceInviteDTO();
            var userMock = UserTestData.GetTestUser();

            SetupGetUserByIdAsync(ownerId, userMock);

            Func<Task> act = () => _workspaceService
                .SendInviteAsync(workspaceInviteDTOMock, ownerId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.Invalid2FVCode);
        }

        protected void SetupMap<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapperMock
                .Setup(x => x.Map(source ?? It.IsAny<TSource>(), destination))
                .Returns(destination)
                .Verifiable();
        }

        protected void SetupGetWorkspaceByKeyAsync(int workspaceId, Workspace workspaceInstance)
        {
            _workspaceRepositoryMock
                .Setup(x => x.GetByKeyAsync(workspaceId))
                .Returns(Task.FromResult(workspaceInstance))
                .Verifiable();
        }

        protected void SetupGetUserByIdAsync(string userId, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId ?? It.IsAny<string>()))
                .ReturnsAsync(userInstance)
                .Verifiable();
        }

        protected void SetupUpdateWorkspaceAsync()
        {
            _workspaceRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Workspace>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupAddWorkspaceAsync(Workspace workspaceInstance)
        {
            _workspaceRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Workspace>()))
                .ReturnsAsync(workspaceInstance)
                .Verifiable();
        }

        protected void SetupAddUserWorkspaceAsync(UserWorkspace userWorkspaceInstance)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<UserWorkspace>()))
                .ReturnsAsync(userWorkspaceInstance)
                .Verifiable();
        }

        protected void SetupWorkspaceSaveChangesAsync()
        {
            _workspaceRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();
        }

        protected void SetupUserWorkspaceSaveChangesAsync()
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();
        }

        //protected void SetupCounterIncrement(Workspace workspaceInstance)
        //{
        //    _workspaceRepositoryMock
        //        .Setup(x => x.AddAsync(workspaceInstance))
        //        .Verifiable();
        //}

        //protected void SetupMembersCountByWorkspaceRole(Workspace workspaceInstance)
        //{
        //    _workspaceRepositoryMock
        //        .Setup(x => x.AddAsync(workspaceInstance))
        //        .Verifiable();
        //}
    }
}
