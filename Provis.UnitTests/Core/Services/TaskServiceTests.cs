﻿using App.Metrics;
using App.Metrics.Counter;
using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
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
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
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
        protected Mock<IOptions<AttachmentSettings>> _attachmentsSettingsOptionsMock;
        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IOptions<ClientUrl>> _clientUrlOptionsMock;
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
            _attachmentsSettingsOptionsMock = new Mock<IOptions<AttachmentSettings>>();
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _clientUrlOptionsMock = new Mock<IOptions<ClientUrl>>();
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
                _clientUrlOptionsMock.Object,
                _emailSendServiceMock.Object,
                _imageSettingsOptionsMock.Object,
                _metricsMock.Object);
        }

        [Test]
        [TestCaseSource("TaskStatusesCase")]
        public async Task GetTaskStatuses_StatusesExists_ReturnTaskStatuses(List<Status> statuses)
        {
            var expectedStatusList = statuses
                .Select(x => new TaskStatusDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            SetupStatusesGetAllAsync(statuses);
            _mapperMock.SetupListToListMap(statuses, expectedStatusList);

            var result = await _taskService.GetTaskStatuses();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedStatusList);
        }

        [Test]
        [TestCaseSource("WorkerRolesCase")]
        public async Task GetWorkerRoles_RolesExists_ReturnWorkerRoles(List<UserRoleTag> workerRoles)
        {
            var expectedRolesList = workerRoles
                .Select(x => new TaskRoleDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            SetupRolesGetAllAsync(workerRoles);
            _mapperMock.SetupListToListMap(workerRoles, expectedRolesList);

            var result = await _taskService.GetWorkerRoles();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRolesList);
        }

        [Test]
        public async Task GetStatusHistories_ValidTaskId_ReturnHistories()
        {
            var historyMock = WithStatusHistories;
            var expectedHistoryList = WithStatusHistoryDTOs;
            var taskId = 1;

            SetupHistoryGetBySpecAsync(historyMock);
            _mapperMock.SetupMap(historyMock, expectedHistoryList);

            var result = await _taskService.GetStatusHistories(taskId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedHistoryList);
        }

        [Test]
        public async Task GetTaskInfo_TaskFound_ReturnTaskInfo()
        {
            var taskId = 1;
            var taskMock = WithWorkspaceTask;
            var expectedTask = new TaskInfoDTO()
            {
                Name = taskMock.Name,
                Deadline = taskMock.DateOfEnd,
                Description = taskMock.Description,
                StatusId = taskMock.StatusId,
                StoryPoints = taskMock.StoryPoints
            };

            SetupTaskGetFirstBySpecAsync(taskMock);
            _mapperMock.SetupMap(taskMock, expectedTask);

            var result = await _taskService.GetTaskInfoAsync(taskId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);
        }

        [Test]
        public async Task GetTaskInfo_TaskNotFound_ThrowHttpException()
        {
            var taskId = 2;
            WorkspaceTask task = null;
            SetupTaskGetFirstBySpecAsync(task);

            Func<Task<TaskInfoDTO>> act = () => _taskService.GetTaskInfoAsync(taskId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        public async Task JoinTask_TaskNotFound_ThrowHttpException()
        {
            var userId = "2";
            WorkspaceTask taskMock = null;
            var taskAssign = WithAssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        public async Task JoinTask_NoPermission_ThrowHttpException()
        {
            var userId = "1";
            var taskMock = WithWorkspaceTask;
            var taskAssign = WithAssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermissionAssignUser);
        }

        [Test]
        [TestCaseSource("JoinTaskNotMemberCase")]
        public async Task JoinTask_UserNotMember_ThrowHttpException(string userId, bool isMember)
        {
            var taskMock = WithWorkspaceTask;
            var taskAssign = WithAssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            SetupUserWorkspaceAnyBySpecAsync(isMember);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserNotMember);
        }

        [Test]
        [TestCaseSource("JoinTaskAssignedCase", new object[] { "2", true, true })]
        public async Task JoinTask_AlreadyAssigned_ThrowHttpException(string userId,
                bool isMember,
                bool assigned)
        {
            var taskMock = WithWorkspaceTask;
            var taskAssign = WithAssignOnTaskDTO;

            SetupJoinTask(taskMock, isMember, assigned);

            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserAlreadyAssigned);
        }

        [Test]
        [TestCaseSource("JoinTaskAssignedCase", new object[] { "2", true, false })]
        public async Task JoinTask_AssignUser_ReturnTaskComplete(string userId, bool isMember, bool assigned)
        {
            var taskMock = WithWorkspaceTask;
            var taskAssignDTO = WithAssignOnTaskDTO;
            var taskAssign = new UserTask()
            {
                TaskId = taskAssignDTO.Id,
                IsUserDeleted = false,
                Task = taskMock,
                UserId = taskAssignDTO.AssignedUser.UserId,
                UserRoleTagId = taskAssignDTO.AssignedUser.RoleTagId
            };

            SetupJoinTask(taskMock, isMember, assigned);
            SetupUserTaskSaveChangesAsync();
            SetupUserTaskAddAsync(taskAssign);
            SetupMetricsIncrement();
            _mapperMock.SetupMap(taskAssignDTO, taskAssign);

            var result = _taskService.JoinTaskAsync(taskAssignDTO, userId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCaseSource("ChangeStatusCase")]
        public async Task ChangeStatus_TaskNotFound_ThrowHttpException(string userId, TaskChangeStatusDTO changeDTO)
        {
            WorkspaceTask taskMock = null;
            var taskAssign = WithAssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.ChangeTaskStatusAsync(changeDTO, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCaseSource("ChangeStatusCase")]
        public async Task ChangeStatus_WithMailing_ReturnTaskComplete(string userId, TaskChangeStatusDTO changeDTO)
        {
            var taskMock = WithWorkspaceTask;
            var userMock = WithUser;
            var workspaceMock = WithWorkspace;
            var statusHistoryMock = WithStatusHistories[1];

            SetupChangeInfo(userId, userMock, taskMock, WithEmails, workspaceMock, WithClientUrl);
            SetupChangeStatusSendManyMailsASync();
            SetupMetricsIncrement();
            SetupMetricsDecrement();
            SetupHistoryAddAsync(statusHistoryMock);

            var result = Task.Run(() =>
                _taskService.ChangeTaskStatusAsync(changeDTO, userId));
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCaseSource("ChangeTaskInfoCase")]
        public async Task ChangeTaskInfo_TaskNotFound_ThrowHttpException(string userId, TaskChangeInfoDTO changeInfoDTO)
        {
            WorkspaceTask taskMock = null;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.ChangeTaskInfoAsync(changeInfoDTO, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCaseSource("ChangeTaskInfoCase")]
        public async Task ChangeTaskInfo_NoPermission_ThrowHttpException(string userId, TaskChangeInfoDTO changeInfoDTO)
        {
            var taskMock = WithWorkspaceTask;
            var havePermission = false;

            SetupTaskGetByKeyAsync(taskMock);
            SetupUserWorkspaceAllBySpecAsync(havePermission);

            Func<Task> act = () => _taskService.ChangeTaskInfoAsync(changeInfoDTO, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermission);
        }

        [Test]
        [TestCaseSource("ChangeTaskInfoCase")]
        public async Task ChangeTaskInfo_WithMailing_ReturnTaskComplete(string userId, TaskChangeInfoDTO changeInfoDTO)
        {
            var taskMock = WithWorkspaceTask;
            var userMock = WithUser;
            var workspaceMock = WithWorkspace;
            var havePermission = true;
            var expectedTask = new WorkspaceTask()
            {
                Id = changeInfoDTO.Id,
                Name = changeInfoDTO.Name,
                DateOfEnd = changeInfoDTO.Deadline,
                Description = changeInfoDTO.Description,
                StoryPoints = changeInfoDTO.StoryPoints,
                WorkspaceId = changeInfoDTO.WorkspaceId
            };

            SetupChangeInfo(userId, userMock, taskMock, WithEmails, workspaceMock, WithClientUrl);
            SetupUserWorkspaceAllBySpecAsync(havePermission);
            SetupChangeTaskSendManyMailsASync();
            _mapperMock.Setup(m =>
                m.Map<TaskChangeInfoDTO, WorkspaceTask>(changeInfoDTO)
                ).Returns(taskMock);

            var result = Task.Run(() =>
                _taskService.ChangeTaskInfoAsync(changeInfoDTO, userId)
            );
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCaseSource("CreateTaskCase")]
        public async Task CreateTask_WorkspaceNotFound_ThrowHttpException(string userId, TaskCreateDTO taskCreate)
        {
            Workspace workspaceMock = null;

            SetupWorkspaceGetByKeyASync(workspaceMock);

            Func<Task> act = () => _taskService.CreateTaskAsync(taskCreate, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.WorkspaceNotFound);
        }

        [Test]
        [TestCaseSource("CreateTaskCase")]
        public async Task CreateTask_UserNotMember_ThrowHttpException(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = WithWorkspace;
            UserWorkspace userWorkspaceMock = null;

            SetupWorkspaceGetByKeyASync(workspaceMock);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspaceMock);

            Func<Task> act = () => _taskService.CreateTaskAsync(taskCreate, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.UserNotMember);
        }

        [Test]
        [TestCaseSource("CreateTaskCase")]
        public async Task CreateTask_TransactionFailed_ThrowHttpException(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = WithWorkspace;
            var userWorkspaceMock = WithUserWorkspace;
            var taskMock = WithWorkspaceTask;
            taskCreate.AssignedUsers.Add(
                new UserAssignedOnTaskDTO()
                {
                    UserId = "1",
                    RoleTagId = 1
                });

            SetupCreateTask(workspaceMock, userWorkspaceMock, taskMock);
            SetupTransactionRollback();
            _mapperMock.Setup(m =>
                m.Map<TaskCreateDTO, WorkspaceTask>(taskCreate)
                ).Returns(taskMock);

            Func<Task> act = () => _taskService.CreateTaskAsync(taskCreate, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.TransactionFailed);
        }

        [Test]
        [TestCaseSource("CreateTaskCase")]
        public async Task CreateTask_TransactionCompleted_ReturnCompletedTask(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = WithWorkspace;
            var userWorkspaceMock = WithUserWorkspace;
            var taskMock = WithWorkspaceTask;

            SetupCreateTask(workspaceMock, userWorkspaceMock, taskMock);
            SetupUserTaskAddRangeAsync();
            SetupUserTaskSaveChangesAsync();
            SetupTransactionCommitAsync();
            _mapperMock.Setup(m =>
                m.Map<TaskCreateDTO, WorkspaceTask>(taskCreate)
                ).Returns(taskMock);

            var result = _taskService.CreateTaskAsync(taskCreate, userId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task GetTasks_UserIdNull_ReturnTaskGroupDTO()
        {
            string userId = null;
            var sprintId = 1;
            var workspace = WithWorkspace;
            var taskMock = WithWorkspaceTasks;
            var taskDTO = WithTaskDTOs;
            var expectedList = WithGetTasks(userId, taskDTO);

            SetupWorkspaceGetByKeyASync(workspace);
            SetupTaskGetListBySpecAsync(taskMock);
            _mapperMock.SetupListToDictionaryMap(taskDTO, taskMock);

            var result = await _taskService.GetTasks(userId, workspace.Id, sprintId);

            result
                .Should()
                .BeEquivalentTo(expectedList);
        }

        [Test]
        public async Task GetTasks_UserIdExist_ReturnTaskGroupDTO()
        {
            var sprintId = 1;
            string userId = "2";
            var workspace = WithWorkspace;
            var taskMock = WithUserTasks;
            var taskDTO = WithTaskDTOs;
            var expectedList = WithGetTasks(userId, taskDTO);

            SetupWorkspaceGetByKeyASync(workspace);
            SetupUserTaskGetListBySpecAsync(taskMock);
            _mapperMock.SetupListToDictionaryMap(taskDTO, taskMock);

            var result = await _taskService.GetTasks(userId, workspace.Id, sprintId);

            result
                .Should()
                .BeEquivalentTo(expectedList);
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
            _clientUrlOptionsMock.Verify();
            _emailSendServiceMock.Verify();
            _imageSettingsOptionsMock.Verify();
            _metricsMock.Verify();
        }

        protected void SetupStatusesGetAllAsync(IEnumerable<Status> statuses)
        {
            _taskStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(statuses)
                .Verifiable();
        }

        protected void SetupRolesGetAllAsync(IEnumerable<UserRoleTag> roles)
        {
            _userRoleTagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(roles)
                .Verifiable();
        }

        protected void SetupHistoryGetBySpecAsync(IEnumerable<StatusHistory> histories)
        {
            _statusHistoryRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<StatusHistory>>()))
                .ReturnsAsync(histories)
                .Verifiable();
        }

        protected void SetupTaskGetFirstBySpecAsync(WorkspaceTask task)
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.GetFirstBySpecAsync(It.IsAny<ISpecification<WorkspaceTask>>()))
                .ReturnsAsync(task)
                .Verifiable();
        }

        protected void SetupTaskGetByKeyAsync(WorkspaceTask task)
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.GetByKeyAsync(It.IsAny<int>()))
                .ReturnsAsync(task)
                .Verifiable();
        }

        protected void SetupUserWorkspaceAnyBySpecAsync(bool state)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.AnyBySpecAsync(It.IsAny<ISpecification<UserWorkspace>>()))
                .ReturnsAsync(state)
                .Verifiable();
        }

        protected void SetupUserTaskAnyBySpecAsync(bool state)
        {
            _userTaskRepositoryMock
                .Setup(x => x.AnyBySpecAsync(It.IsAny<ISpecification<UserTask>>()))
                .ReturnsAsync(state)
                .Verifiable();
        }

        protected void SetupUserTaskSaveChangesAsync()
        {
            _userTaskRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupUserTaskAddAsync(UserTask userTask)
        {
            _userTaskRepositoryMock
                .Setup(x => x.AddAsync(userTask))
                .ReturnsAsync(userTask)
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

        protected void SetupUserGetByKeyAsync(string userId, User user)
        {
            _userRepositoryMock
                .Setup(x => x.GetByKeyAsync(userId))
                .ReturnsAsync(user)
                .Verifiable();
        }

        protected void SetupWorkspaceGetByKeyASync(Workspace workspace)
        {
            _workspaceRepositoryMock
                .Setup(x => x.GetByKeyAsync(It.IsAny<int>()))
                .ReturnsAsync(workspace)
                .Verifiable();
        }

        protected void SetupUserTaskEmailsGetListBySpecAsync(List<string> emails)
        {
            _userTaskRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<UserTask, string>>()))
                .ReturnsAsync(emails)
                .Verifiable();
        }

        protected void SetupTaskGetListBySpecAsync(
            IEnumerable<Tuple<int, WorkspaceTask, int, int, string>> tasks)
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.GetListBySpecAsync(
                    It.IsAny<ISpecification<WorkspaceTask,
                    Tuple<int, WorkspaceTask, int, int, string>>>()))
                .ReturnsAsync(tasks)
                .Verifiable();
        }

        protected void SetupChangeTaskSendManyMailsASync()
        {
            _emailSendServiceMock
                .Setup(x => x.SendManyMailsAsync(It.IsAny<MailingRequest<TaskEdited>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupChangeStatusSendManyMailsASync()
        {
            _emailSendServiceMock
                .Setup(x => x.SendManyMailsAsync(It.IsAny<MailingRequest<TaskChangeStatus>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupHistoryAddAsync(StatusHistory history)
        {
            _statusHistoryRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<StatusHistory>()))
                .ReturnsAsync(history)
                .Verifiable();
        }

        protected void SetupTaskUpdateAsync()
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<WorkspaceTask>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupTaskSaveChangesAsync()
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupClientUrlOptions(ClientUrl url)
        {
            _clientUrlOptionsMock
                .Setup(x => x.Value)
                .Returns(url)
                .Verifiable();
        }

        protected void SetupUserWorkspaceAllBySpecAsync(bool state)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.AllBySpecAsync(
                        It.IsAny<ISpecification<UserWorkspace>>(),
                        It.IsAny<Expression<Func<UserWorkspace, bool>>>()))
                .ReturnsAsync(state)
                .Verifiable();
        }

        protected void SetupUserWorkspaceGetFirstBySpecAsync(UserWorkspace userWorkspace)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.GetFirstBySpecAsync(It.IsAny<ISpecification<UserWorkspace>>()))
                .ReturnsAsync(userWorkspace)
                .Verifiable();
        }

        protected void SetupTaskAddAsync(WorkspaceTask workspaceTask)
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<WorkspaceTask>()))
                .ReturnsAsync(workspaceTask)
                .Verifiable();
        }

        protected void SetupUserTaskAddRangeAsync()
        {
            _userTaskRepositoryMock
                .Setup(x => x.AddRangeAsync(It.IsAny<List<UserTask>>()))
                .Verifiable();
        }

        protected void SetupUserTaskGetListBySpecAsync(
            IEnumerable<Tuple<int, UserTask, int, int, string>> tasks)
        {
            _userTaskRepositoryMock
                .Setup(x => x.GetListBySpecAsync(
                    It.IsAny<ISpecification<UserTask, Tuple<int, UserTask, int, int, string>>>()))
                .ReturnsAsync(tasks)
                .Verifiable();
        }

        protected void SetupTransactionCommitAsync()
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.BeginTransactionAsync(IsolationLevel.RepeatableRead)
                    .Result.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupTransactionRollback()
        {
            _workspaceTaskRepositoryMock
                .Setup(x => x.BeginTransactionAsync(IsolationLevel.RepeatableRead)
                    .Result.Rollback())
                .Verifiable();
        }

        private void SetupJoinTask(WorkspaceTask task, bool isMember, bool isAssigned)
        {
            SetupTaskGetByKeyAsync(task);
            SetupUserWorkspaceAnyBySpecAsync(isMember);
            SetupUserTaskAnyBySpecAsync(isAssigned);
        }

        private void SetupChangeInfo(string userId,
                User user,
                WorkspaceTask task,
                List<string> emails,
                Workspace workspace,
                ClientUrl url)
        {
            SetupUserGetByKeyAsync(userId, user);
            SetupTaskGetByKeyAsync(task);
            SetupWorkspaceGetByKeyASync(workspace);
            SetupUserTaskEmailsGetListBySpecAsync(emails);
            SetupClientUrlOptions(url);
            SetupTaskUpdateAsync();
            SetupTaskSaveChangesAsync();
        }

        private void SetupCreateTask(Workspace workspace, UserWorkspace userWorkspace, WorkspaceTask task)
        {
            SetupWorkspaceGetByKeyASync(workspace);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspace);
            SetupMetricsIncrement();
            SetupTaskAddAsync(task);
            SetupTaskSaveChangesAsync();
        }

        private User WithUser
        {
            get
            {
                return new User()
                {
                    Id = "2",
                    Email = "test1@gmail.com",
                    Name = "Name1",
                    Surname = "Surname1",
                    UserName = "Username1",
                    ImageAvatarUrl = "Path1"
                };
            }
        }

        private UserWorkspace WithUserWorkspace
        {
            get
            {
                return new UserWorkspace()
                {
                    UserId = "1",
                    RoleId = 1,
                    WorkspaceId = 2
                };
            }
        }

        private Workspace WithWorkspace
        {
            get
            {
                return new Workspace()
                {
                    Id = 1,
                    DateOfCreate = DateTime.Now,
                    Description = "Description mock",
                    Name = "Provis",
                    isUseSprints = false
                };
            }
        }

        private WorkspaceTask WithWorkspaceTask
        {
            get
            {
                return new WorkspaceTask()
                {
                    Id = 2,
                    Name = "Create workspace",
                    DateOfCreate = DateTimeOffset.UtcNow,
                    DateOfEnd = DateTimeOffset.UtcNow,
                    Description = "Nope",
                    StatusId = 1,
                    WorkspaceId = 1,
                    TaskCreatorId = "2"
                };
            }
        }

        private static IEnumerable<TestCaseData> TaskStatusesCase
        {
            get
            {
                yield return new TestCaseData(new List<Status>()
                {
                    new Status()
                    {
                       Id = 1,
                       Name = "In review"
                    },
                    new Status()
                    {
                       Id = 2,
                       Name = "Completed"
                    },
                    new Status()
                    {
                       Id = 3,
                       Name = "Closed"
                    }
                });
            }
        }

        private static IEnumerable<TestCaseData> WorkerRolesCase
        {
            get
            {
                yield return new TestCaseData(new List<UserRoleTag>()
                {
                    new UserRoleTag()
                    {
                        Id=1,
                        Name = "Developer"
                    },
                    new UserRoleTag()
                    {
                        Id=2,
                        Name = "Visitor"
                    },
                    new UserRoleTag()
                    {
                        Id=3,
                        Name = "Project manager"
                    }
                });
            }
        }

        private List<StatusHistory> WithStatusHistories
        {
            get
            {
                return new List<StatusHistory>()
                {
                    new StatusHistory()
                    {
                        Id = 1,
                        DateOfChange = DateTimeOffset.UtcNow,
                        TaskId = 1,
                        StatusId = 1,
                        UserId = "1"
                    },
                    new StatusHistory()
                    {
                        Id = 2,
                        DateOfChange = DateTimeOffset.UtcNow,
                        TaskId = 2,
                        StatusId = 2,
                        UserId = "2"
                    },
                    new StatusHistory()
                    {
                        Id = 3,
                        DateOfChange = DateTimeOffset.UtcNow,
                        TaskId = 1,
                        StatusId = 3,
                        UserId = "4"
                    }
                };
            }
        }

        private List<TaskStatusHistoryDTO> WithStatusHistoryDTOs
        {
            get
            {
                return new List<TaskStatusHistoryDTO>()
                {
                    new TaskStatusHistoryDTO()
                    {
                        DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                        Status = "Completed",
                        StatusId = 1,
                        UserId = "1",
                        UserName = "Artem"
                    },
                    new TaskStatusHistoryDTO()
                    {
                        DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                        Status = "Completed",
                        StatusId = 1,
                        UserId = "3",
                        UserName = "Nazar"
                    },
                    new TaskStatusHistoryDTO()
                    {
                        DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                        Status = "In review",
                        StatusId = 3,
                        UserId = "4",
                        UserName = "Vasyl"
                    }
                };
            }
        }

        private TaskAssignDTO WithAssignOnTaskDTO
        {
            get
            {
                return new TaskAssignDTO()
                {
                    Id = 1,
                    WorkspaceId = 2,
                    AssignedUser = new UserAssignedOnTaskDTO()
                    {
                        UserId = "2",
                        RoleTagId = 2
                    }
                };
            }
        }

        private static IEnumerable<TestCaseData> ChangeStatusCase
        {
            get
            {
                yield return new TestCaseData(
                    "2",
                    new TaskChangeStatusDTO()
                    {
                        WorkspaceId = 1,
                        TaskId = 1,
                        StatusId = 2
                    });
            }
        }

        private List<string> WithEmails
        {
            get
            {
                return new List<string>()
                {
                    "test@gmail.com",
                    "test1@gmail.com",
                    "test2@gmail.com",
                    "test3@gmail.com",
                    "test4@gmail.com",
                };
            }
        }

        private ClientUrl WithClientUrl
        {
            get
            {
                return new ClientUrl()
                {
                    ApplicationUrl = new Uri("http://localhost:4200/")
                };
            }
        }

        private static IEnumerable<TestCaseData> ChangeTaskInfoCase
        {
            get
            {
                yield return new TestCaseData(
                    "3",
                    new TaskChangeInfoDTO()
                    {
                        Id = 1,
                        Deadline = DateTimeOffset.UtcNow,
                        Description = "New description",
                        Name = "New task",
                        StoryPoints = 3,
                        WorkspaceId = 1
                    });
            }
        }

        private static IEnumerable<TestCaseData> CreateTaskCase
        {
            get
            {
                yield return new TestCaseData(
                    "3",
                    new TaskCreateDTO()
                    {
                        Name = "Provis",
                        DateOfEnd = DateTimeOffset.UtcNow,
                        Description = "Create description",
                        StatusId = 1,
                        WorkspaceId = 2,
                        AssignedUsers = new List<UserAssignedOnTaskDTO>()
                        {
                            new UserAssignedOnTaskDTO()
                            {
                               UserId = "1",
                               RoleTagId = 1
                            }
                        }
                    });
            }
        }

        private List<Tuple<int, WorkspaceTask, int, int, string>> WithWorkspaceTasks
        {
            get
            {
                return new List<Tuple<int, WorkspaceTask, int, int, string>>()
                {
                    new Tuple<int, WorkspaceTask, int, int, string>(
                        1,
                        new WorkspaceTask()
                        {
                            Id = 1,
                            Name = "TestTask",
                            DateOfCreate = DateTimeOffset.UtcNow,
                            DateOfEnd = DateTimeOffset.UtcNow,
                            Description = "Test description",
                            StatusId = 1,
                            StoryPoints = 3,
                            TaskCreatorId = "1",
                            WorkspaceId = 2
                        },
                        3,4,"Test1"),
                    new Tuple<int, WorkspaceTask, int, int, string>(
                        2,
                        new WorkspaceTask()
                        {
                            Id = 1,
                            Name = "TestTask",
                            DateOfCreate = DateTimeOffset.UtcNow,
                            DateOfEnd = DateTimeOffset.UtcNow,
                            Description = "Test description",
                            StatusId = 1,
                            StoryPoints = 3,
                            TaskCreatorId = "1",
                            WorkspaceId = 2
                        },
                        3,4,"Test1")
                };
            }
        }

        private Dictionary<int, List<TaskDTO>> WithTaskDTOs
        {
            get
            {
                return new Dictionary<int, List<TaskDTO>>()
                {
                    { 1, new List<TaskDTO>()
                    {
                        new TaskDTO()
                        {
                            Id = 1,
                            CommentCount = 1,
                            CreatorUsername ="Test",
                            Deadline = DateTimeOffset.UtcNow,
                            MemberCount = 3,
                            Name = "TestTask",
                            StoryPoints = 3,
                            WorkerRoleId = 2
                        }
                    }
                    },
                    { 2, new List<TaskDTO>()
                    {
                        new TaskDTO()
                        {
                            Id = 1,
                            CommentCount = 1,
                            CreatorUsername ="Test",
                            Deadline = DateTimeOffset.UtcNow,
                            MemberCount = 3,
                            Name = "TestTask",
                            StoryPoints = 3,
                            WorkerRoleId = 2
                        }
                    }
                    }
                };
            }
        }

        private List<Tuple<int, UserTask, int, int, string>> WithUserTasks
        {
            get
            {
                return new List<Tuple<int, UserTask, int, int, string>>()
                {
                    new Tuple<int, UserTask, int, int, string>(
                        1,
                        new UserTask()
                        {
                            IsUserDeleted = false,
                            TaskId = 1,
                            UserId = "1",
                            UserRoleTagId = 1
                        },
                        3,4,"Test"),
                    new Tuple<int, UserTask, int, int, string>(
                        2,
                        new UserTask()
                        {
                            IsUserDeleted = false,
                            TaskId = 1,
                            UserId = "1",
                            UserRoleTagId = 1
                        },
                        3,4,"Test")
                };
            }
        }

        private TaskGroupByStatusDTO WithGetTasks(string userId, Dictionary<int, List<TaskDTO>> tasks)
        {
            return new TaskGroupByStatusDTO()
            {
                UserId = userId,
                Tasks = tasks
            };
        }

        private static IEnumerable<TestCaseData> JoinTaskNotMemberCase()
        {
            yield return new TestCaseData("2", false);
        }

        private static IEnumerable<TestCaseData> JoinTaskAssignedCase(string id,
                bool isMember,
                bool assigned)
        {
            yield return new TestCaseData(id, isMember, assigned);
        }

        public static IEnumerable<TestCaseData> GetTasks(string userId)
        {
            yield return new TestCaseData(userId, 1);
        }
    }
}
