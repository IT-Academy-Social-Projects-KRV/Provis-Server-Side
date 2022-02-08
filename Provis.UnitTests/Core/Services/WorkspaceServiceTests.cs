using App.Metrics;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
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
using Provis.UnitTests.Resources;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Ardalis.Specification;
using App.Metrics.Counter;
using System.Linq.Expressions;
using Provis.Core.Helpers.Mails.ViewModels;
using Provis.Core.Roles;

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
        protected Mock<IOptions<ClientUrl>> _clientUrlMock;

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
            _clientUrlMock = new Mock<IOptions<ClientUrl>>();

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
                _clientUrlMock.Object,
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

            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[0];

            SetupMetricsIncrement();

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
        public async Task SendInviteAsync_SendInviteYourself_ThrowHTTPException(string ownerId)
        {
            var workspaceInviteDTOMock = WorkspaceTestData.GetTestWorkspaceInviteDTO();
            var userMock = UserTestData.GetTestUser();

            SetupGetUserByIdAsync(ownerId, userMock);

            Func<Task> act = () => _workspaceService
                .SendInviteAsync(workspaceInviteDTOMock, ownerId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.SendInviteYourself);
        }

        [Test]
        [TestCase("2")]
        public async Task SendInviteAsync_UserAcceptedInvite_ThrowHTTPException(string ownerId)
        {
            var workspaceInviteDTOMock = WorkspaceTestData.GetTestWorkspaceInviteDTO();
            var userOwnerMock = UserTestData.GetTestUserList()[1];
            SetupGetUserByIdAsync(ownerId, userOwnerMock);

            var userInviteMock = UserTestData.GetTestUser();
            SetupGetUserByEmailAsync(workspaceInviteDTOMock.UserEmail, userInviteMock);

            var workspaceMock = WorkspaceTestData.GetTestWorkspace();
            SetupGetWorkspaceByKeyAsync(workspaceInviteDTOMock.WorkspaceId, workspaceMock);

            bool isConfirm = true;
            SetupAnyBySpecAsync(isConfirm);

            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[0];
            SetupGetFirstBySpecAsync(userWorkspaceMock);

            Func<Task> act = () => _workspaceService
                .SendInviteAsync(workspaceInviteDTOMock, ownerId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserAcceptedInvite);
        }

        [Test]
        [TestCase("1")]
        public async Task SendInviteAsync_UserAlredyHasInvite_ThrowHTTPException(string ownerId)
        {
            var workspaceInviteDTOMock = WorkspaceTestData.GetTestWorkspaceInviteDTO();
            var userOwnerMock = UserTestData.GetTestUserList()[1];
            SetupGetUserByIdAsync(ownerId, userOwnerMock);

            var userInviteMock = UserTestData.GetTestUser();
            SetupGetUserByEmailAsync(workspaceInviteDTOMock.UserEmail, userInviteMock);

            var workspaceMock = WorkspaceTestData.GetTestWorkspace();
            SetupGetWorkspaceByKeyAsync(workspaceInviteDTOMock.WorkspaceId, workspaceMock);

            bool isConfirm = true;
            SetupAnyBySpecAsync(isConfirm);

            UserWorkspace userNull = null;
            SetupGetFirstBySpecAsync(userNull);

            Func<Task> act = () => _workspaceService
                .SendInviteAsync(workspaceInviteDTOMock, ownerId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserAlreadyHasInvite);
        }

        [Test]
        [TestCase("2")]
        public async Task SendInviteAsync_InviteIsValid_ReturnCompletedTask(string ownerId)
        {
            var workspaceInviteDTOMock = WorkspaceTestData.GetTestWorkspaceInviteDTO();
            var userOwnerMock = UserTestData.GetTestUserList()[1];
            SetupGetUserByIdAsync(ownerId, userOwnerMock);

            var userInviteMock = UserTestData.GetTestUser();
            SetupGetUserByEmailAsync(workspaceInviteDTOMock.UserEmail, userInviteMock);

            var workspaceMock = WorkspaceTestData.GetTestWorkspace();
            SetupGetWorkspaceByKeyAsync(workspaceInviteDTOMock.WorkspaceId, workspaceMock);

            bool isConfirm = false;
            SetupAnyBySpecAsync(isConfirm);

            var inviteUserMock = InviteUserTestData.GetInviteUserList()[0];
            SetupAddInviteUserAsync(inviteUserMock);
            SetupInviteUserSaveChangesAsync();

            var templateStringMock = "template";
            var viewName = "Mails/WorkspaceInvite";
            var uriStringMock = "http://localhost:4200/";
            Uri uriMock = new(uriStringMock);
            SetupApplicationUrl(new() { ApplicationUrl = uriMock });

            SetupGetTemplateHtmlAsStringAsync(viewName, templateStringMock);
            SetupSendEmailAsync();

            var result = _workspaceService.SendInviteAsync(workspaceInviteDTOMock, ownerId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase(1,"2")]
        public async Task DenyInviteAsync_NotYoursInvite_ThrowHTTPException(int inviteId, string ownerId)
        {
            var inviteUserMock = InviteUserTestData.GetInviteUserList()[0];
            SetupGetInviteUserByKeyAsync(inviteId, inviteUserMock);

            Func<Task> act = () => _workspaceService
                .DenyInviteAsync(inviteId, ownerId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.CannotDenyInvite);
        }

        [Test]
        [TestCase(1, "1")]
        public async Task DenyInviteAsync_InviteIsValid_ReturnCompletedTask(int inviteId, string ownerId)
        {
            var inviteUserMock = InviteUserTestData.GetInviteUserList()[0];
            SetupGetInviteUserByKeyAsync(inviteId, inviteUserMock);

            var result = _workspaceService.DenyInviteAsync(inviteId, ownerId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase("1")]
        public async Task GetWorkspaceListAsync_UserIsValid_ReturnWorkspaceList(string userId)
        {
            var expectedWorkspaceListMock = WorkspaceTestData.GetTestWorkspaceListInfoDTO();
            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList();

            SetupGetWorkspaceListBySpecAsync(userWorkspaceMock);
            _mapperMock.SetupMap(userWorkspaceMock, expectedWorkspaceListMock);

            var result = await _workspaceService.GetWorkspaceListAsync(userId);

            result.Should().NotBeNull();
            result.Should().Equal(expectedWorkspaceListMock);
        }

        [Test]
        [TestCase(1, "2")]
        public async Task AcceptInviteAsync_CannotAcceptInvite_ThrowHTTPException(int inviteId, string userId)
        {
            var inviteUserMock = InviteUserTestData.GetInviteUserList()[0];
            SetupGetInviteUserByKeyAsync(inviteId, inviteUserMock);

            Func<Task> act = () => _workspaceService
                .AcceptInviteAsync(inviteId, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.CannotAcceptInvite);
        }

        [Test]
        [TestCase(1, "1")]
        public async Task AcceptInviteAsync_InviteAlredyAccepted_ThrowHTTPException(int inviteId, string userId)
        {
            var inviteUserMock = InviteUserTestData.GetInviteUserList()[0];
            SetupGetInviteUserByKeyAsync(inviteId, inviteUserMock);

            Func<Task> act = () => _workspaceService
                .AcceptInviteAsync(inviteId, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.InviteAlreadyAccerted);
        }

        [Test]
        [TestCase(2, "1")]
        public async Task AcceptInviteAsync_InviteIsValid_ReturnCompletedTask(int inviteId, string ownerId)
        {
            var inviteUserMock = InviteUserTestData.GetInviteUserList()[1];
            SetupGetInviteUserByKeyAsync(inviteId, inviteUserMock);

            var userTasksMock = UserTaskTestData.GetWorkspaceUserTasks();
            SetupGetUserTaskListBySpecAsync(userTasksMock);

            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[0];
            SetupMetricsIncrement();
            SetupAddUserWorkspaceAsync(userWorkspaceMock);
            SetupInviteUserSaveChangesAsync();

            var result = _workspaceService.AcceptInviteAsync(inviteId, ownerId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase(1, "1")]
        public async Task GetWorkspaceInfoAsync_UserIsValid_ReturnWorkspaceInfo(int workspaceId, string userId)
        {
            var expectedWorkspaceMock = WorkspaceTestData.GetTestWorkspaceListInfoDTO()[0];
            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[0];

            SetupGetFirstBySpecAsync(userWorkspaceMock);
            _mapperMock.SetupMap(userWorkspaceMock, expectedWorkspaceMock);

            var result = await _workspaceService.GetWorkspaceInfoAsync(workspaceId, userId);

            result.Should().NotBeNull();
            result.Should().Be(expectedWorkspaceMock);
        }

        [Test]
        [TestCase("2")]
        public async Task ChangeUserRoleAsync_HasNotPermissions_ThrowHTTPException(string userId)
        {
            var workspaceChangeRoleDTOMock = WorkspaceTestData.GetTestWorkspaceChangeRoleDTO();
            var userWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[1];

            var targetWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[2];
            SetupGetFirstBySpecAsync(targetWorkspaceMock);

            var roleAccess = GetTestRoleAccess();
            var dictionaryAccess = roleAccess.RolesAccess;
                
            _roleAccessMock.Object.RolesAccess = dictionaryAccess;

            Func<Task> act = () => _workspaceService
                .ChangeUserRoleAsync(userId, workspaceChangeRoleDTOMock);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermission);
        }

        [Test]
        [TestCase("3")]
        public async Task ChangeUserRoleAsync_UsersIsValid_ReturnChangeRoleDTO(string userId)
        {
            var workspaceChangeRoleDTOMock = WorkspaceTestData.GetTestWorkspaceChangeRoleDTO2();
            var targetUserWorkspaceMock = UserWorkspaceTestData.GetTestUserWorkspaceList()[2];
            SetupGetFirstBySpecAsync(targetUserWorkspaceMock);

            var roleAccess = GetTestRoleAccess();
            var dictionaryAccess = roleAccess.RolesAccess;

            _roleAccessMock.Object.RolesAccess = dictionaryAccess;

            SetupMetricsDecrement();
            SetupMetricsIncrement();
            SetupUserWorkspaceSaveChangesAsync();
            _mapperMock.SetupMap(targetUserWorkspaceMock, workspaceChangeRoleDTOMock);
            
            var result = await _workspaceService.ChangeUserRoleAsync(userId, workspaceChangeRoleDTOMock);

            result.Should().NotBeNull();
            result.Should().Be(workspaceChangeRoleDTOMock);
        }

        protected void SetupApplicationUrl(ClientUrl clientUri)
        {
            _clientUrlMock
                .Setup(x => x.Value)
                .Returns(clientUri)
                .Verifiable();
        }

        protected void SetupGetTemplateHtmlAsStringAsync(string viewName,
            string templateInstance)
        {
            _templateServiceMock
                .Setup(x => x.GetTemplateHtmlAsStringAsync(viewName ?? It.IsAny<string>(),
                                                           It.IsAny<WorkspaceInvite>()))
                .ReturnsAsync(templateInstance)
                .Verifiable();
        }

        protected void SetupSendEmailAsync()
        {
            _emailSendServiceMock
                .Setup(x => x.SendEmailAsync(It.IsAny<MailRequest>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupMetricsIncrement()
        {
            _metricsMock
                .Setup(x => x.Measure.Counter.Increment(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>()))
                .Verifiable();
        }

        protected void SetupMetricsDecrement()
        {
            _metricsMock
                .Setup(x => x.Measure.Counter.Decrement(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>()))
                .Verifiable();
        }

        protected void SetupAnyBySpecAsync(bool isConfirm)
        {
            _inviteUserRepositoryMock
                .Setup(x => x.AnyBySpecAsync(It.IsAny<ISpecification<InviteUser>>(), It.IsAny<Expression<Func<InviteUser, bool>>>()))
                .ReturnsAsync(isConfirm)
                .Verifiable();
        }

        protected void SetupGetUserTaskListBySpecAsync(IEnumerable<Tuple<int, UserTask, int, int, string>> tasks)
        {
            _userTaskRepositoryMock
                .Setup(x => x.GetListBySpecAsync(
                    It.IsAny<ISpecification<UserTask, Tuple<int, UserTask, int, int, string>>>()))
                .ReturnsAsync(tasks)
                .Verifiable();
        }

        protected void SetupGetWorkspaceListBySpecAsync(List<UserWorkspace> listInstance)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<UserWorkspace>>()))
                .ReturnsAsync(listInstance)
                .Verifiable();
        }

        protected void SetupGetFirstBySpecAsync(UserWorkspace userWorkspaceInstance)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.GetFirstBySpecAsync(It.IsAny<ISpecification<UserWorkspace>>()))
                .Returns(Task.FromResult(userWorkspaceInstance))
                .Verifiable();
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

        protected void SetupGetInviteUserByKeyAsync(int inviteId, InviteUser inviteUserInstance)
        {
            _inviteUserRepositoryMock
                .Setup(x => x.GetByKeyAsync(inviteId))
                .Returns(Task.FromResult(inviteUserInstance))
                .Verifiable();
        }

        protected void SetupGetUserByIdAsync(string userId, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId ?? It.IsAny<string>()))
                .ReturnsAsync(userInstance)
                .Verifiable();
        }

        protected void SetupGetUserByEmailAsync(string userEmail, User userInstance)
        {
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(userEmail ?? It.IsAny<string>()))
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

        protected void SetupAddInviteUserAsync(InviteUser inviteUserInstance)
        {
            _inviteUserRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<InviteUser>()))
                .ReturnsAsync(inviteUserInstance)
                .Verifiable();
        }

        protected void SetupWorkspaceSaveChangesAsync()
        {
            _workspaceRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();
        }

        protected void SetupInviteUserSaveChangesAsync()
        {
            _inviteUserRepositoryMock
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

        public static RoleAccess GetTestRoleAccess()
        {
            return new RoleAccess()
            {
                RolesAccess = new Dictionary<WorkSpaceRoles, List<WorkSpaceRoles>>() 
                {
                    {
                        WorkSpaceRoles.OwnerId, new List<WorkSpaceRoles>
                        {
                            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId
                        } 
                    },

                    {
                        WorkSpaceRoles.ManagerId, new List<WorkSpaceRoles>
                        {
                            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId
                        } 
                    }
                }
            };
        }
    }
}
