using App.Metrics;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Base.TestData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        protected TaskService _taskService;

        protected Mock<IRepository<User>> _userRepositoryMock;
        protected Mock<IRepository<WorkspaceTask>> _workspaceTaskRepositoryMock;
        protected Mock<IRepository<Workspace>> _workspaceRepositoryMock;
        protected Mock<IRepository<Status>> _taskStatusRepositoryMock;
        protected Mock<IRepository<UserWorkspace>> _userWorkspaceRepositoryMock;
        protected Mock<IMapper> _mapperMock;
        protected Mock<IRepository<StatusHistory>> _statusHistoryRepositoryMock;
        protected Mock<IRepository<UserTask>> _userTaskRepositoryMock;
        protected Mock<IRepository<UserRoleTag>> _userRoleTagRepositoryMock;
        protected Mock<IRepository<WorkspaceTaskAttachment>> _workspaceTaskAttachmentsRepositoryMock;
        protected Mock<IRepository<Comment>> _commentRepositoryMock;
        protected Mock<IFileService> _fileServiceMock;
        protected Mock<IOptions<TaskAttachmentSettings>> _attachmentsSettingsOptionsMock;
        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IOptions<ClientUrl>> _optionsMock;
        protected Mock<IEmailSenderService> _emailSendServiceMock;
        protected Mock<IOptions<ImageSettings>> _imageSettingsOptionsMock;
        protected Mock<IMetrics> _metricsMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _workspaceTaskRepositoryMock = new Mock<IRepository<WorkspaceTask>>();
            _workspaceRepositoryMock = new Mock<IRepository<Workspace>>();
            _taskStatusRepositoryMock = new Mock<IRepository<Status>>();
            _userWorkspaceRepositoryMock = new Mock<IRepository<UserWorkspace>>();
            _mapperMock = new Mock<IMapper>();
            _statusHistoryRepositoryMock = new Mock<IRepository<StatusHistory>>();
            _userTaskRepositoryMock = new Mock<IRepository<UserTask>>();
            _userRoleTagRepositoryMock = new Mock<IRepository<UserRoleTag>>();
            _workspaceTaskAttachmentsRepositoryMock = new Mock<IRepository<WorkspaceTaskAttachment>>();
            _commentRepositoryMock = new Mock<IRepository<Comment>>();
            _fileServiceMock = new Mock<IFileService>();
            _attachmentsSettingsOptionsMock = new Mock<IOptions<TaskAttachmentSettings>>();
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _optionsMock = new Mock<IOptions<ClientUrl>>();
            _emailSendServiceMock = new Mock<IEmailSenderService>();
            _imageSettingsOptionsMock = new Mock<IOptions<ImageSettings>>();
            _metricsMock = new Mock<IMetrics>();

            _taskService = new TaskService(
                _userRepositoryMock.Object,
                _workspaceTaskRepositoryMock.Object,
                _workspaceRepositoryMock.Object,
                _taskStatusRepositoryMock.Object,
                _userWorkspaceRepositoryMock.Object,
                _mapperMock.Object,
                _statusHistoryRepositoryMock.Object,
                _userTaskRepositoryMock.Object,
                _userRoleTagRepositoryMock.Object,
                _workspaceTaskAttachmentsRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _fileServiceMock.Object,
                _attachmentsSettingsOptionsMock.Object,
                _userManagerMock.Object,
                _optionsMock.Object,
                _emailSendServiceMock.Object,
                _imageSettingsOptionsMock.Object,
                _metricsMock.Object);
        }

        [Test]
        public async Task GetTaskStatuses_StatusesExists_ReturnTaskStatuses()
        {
            var statusListMock = TaskTestData.GetTaskStatusesList();
            var expectedStatusList = new List<TaskStatusDTO>();

            expectedStatusList = statusListMock
                .Select(x => new TaskStatusDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            SetupGetAllAsync(statusListMock);
            _mapperMock.SetupMap(statusListMock, expectedStatusList);

            var result = await _taskService.GetTaskStatuses();

            Assert.AreEqual(expectedStatusList, result);
            //result.Should().BeSameAs(expectedStatusList);
        }

        [TearDown]
        public void TearDown()
        {
            _userRepositoryMock.Verify();
            _workspaceTaskRepositoryMock.Verify();
            _workspaceRepositoryMock.Verify();
            _taskStatusRepositoryMock.Verify();
            _userWorkspaceRepositoryMock.Verify();
            _mapperMock.Verify();
            _statusHistoryRepositoryMock.Verify();
            _userTaskRepositoryMock.Verify();
            _userRoleTagRepositoryMock.Verify();
            _workspaceTaskAttachmentsRepositoryMock.Verify();
            _commentRepositoryMock.Verify();
            _fileServiceMock.Verify();
            _attachmentsSettingsOptionsMock.Verify();
            _userManagerMock.Verify();
            _optionsMock.Verify();
            _emailSendServiceMock.Verify();
            _imageSettingsOptionsMock.Verify();
            _metricsMock.Verify();
        }

        protected void SetupGetAllAsync(IEnumerable<Status> statuses)
        {
            _taskStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(statuses))
                .Verifiable();
        }
    }
}
