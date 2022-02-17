using App.Metrics;
using App.Metrics.Counter;
using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.ApiModels;
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
using System.IO;
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
        protected Mock<IOptions<TaskAttachmentSettings>> _attachmentsSettingsOptionsMock;
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
            _attachmentsSettingsOptionsMock = new Mock<IOptions<TaskAttachmentSettings>>();
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
        [TestCaseSource("TaskStatuses")]
        public async Task GetTaskStatuses_StatusesExists_ReturnTaskStatuses(List<Status> statuses)
        {
            var expectedStatusList = statuses
                .Select(x => new TaskStatusDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            SetupStatusesGetAllAsync(statuses);
            for (int i = 0; i < statuses.Count; i++)
                _mapperMock.SetupMap(statuses[i], expectedStatusList[i]);

            var result = await _taskService.GetTaskStatuses();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedStatusList);
        }

        [Test]
        [TestCaseSource("WorkerRoles")]
        public async Task GetWorkerRoles_RolesExists_ReturnWorkerRoles(List<UserRoleTag> workerRoles)
        {
            var expectedRolesList = workerRoles
                .Select(x => new TaskRoleDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            SetupRolesGetAllAsync(workerRoles);
            for (int i = 0; i < workerRoles.Count; i++)
                _mapperMock.SetupMap(workerRoles[i], expectedRolesList[i]);

            var result = await _taskService.GetWorkerRoles();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRolesList);
        }

        [Test]
        [TestCase("1")]
        public async Task GetStatusHistories_ValidTaskId_ReturnHistories(int id)
        {
            var historyMock = StatusHistories;
            var expectedHistoryList = StatusHistoryDTOs;

            SetupHistoryGetBySpecAsync(historyMock);
            _mapperMock.SetupMap(historyMock, expectedHistoryList);

            var result = await _taskService.GetStatusHistories(id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedHistoryList);
        }

        [Test]
        [TestCase("1")]
        public async Task GetTaskInfo_TaskFound_ReturnTaskInfo(int taskId)
        {
            var taskMock = GetWorkspaceTask;
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
        [TestCase("2")]
        public async Task GetTaskInfo_TaskNotFound_ThrowHttpException(int taskId)
        {
            WorkspaceTask task = null;
            SetupTaskGetFirstBySpecAsync(task);

            Func<Task<TaskInfoDTO>> act = () => _taskService.GetTaskInfoAsync(taskId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCase("2")]
        public async Task JoinTask_TaskNotFound_ThrowHttpException(string userId)
        {
            WorkspaceTask taskMock = null;
            var taskAssign = AssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCase("1")]
        public async Task JoinTask_NoPermission_ThrowHttpException(string userId)
        {
            var taskMock = GetWorkspaceTask;
            var taskAssign = AssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermissionAssignUser);
        }

        [Test]
        [TestCase("2", false)]
        public async Task JoinTask_UserNotMember_ThrowHttpException(string userId, bool isMember)
        {
            var taskMock = GetWorkspaceTask;
            var taskAssign = AssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            SetupUserWorkspaceAnyBySpecAsync(isMember);
            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserNotMember);
        }

        [Test]
        [TestCase("2", true, true)]
        public async Task JoinTask_AlreadyAssigned_ThrowHttpException(string userId, bool isMember, bool assigned)
        {
            var taskMock = GetWorkspaceTask;
            var taskAssign = AssignOnTaskDTO;

            SetupJoinTask(taskMock, isMember, assigned);

            Func<Task> act = () => _taskService.JoinTaskAsync(taskAssign, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.UserAlreadyAssigned);
        }

        [Test]
        [TestCase("2", true, false)]
        public async Task JoinTask_AssignUser_ReturnTaskComplete(string userId, bool isMember, bool assigned)
        {
            var taskMock = GetWorkspaceTask;
            var taskAssignDTO = AssignOnTaskDTO;
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
        [TestCaseSource("ChangeStatus")]
        public async Task ChangeStatus_TaskNotFound_ThrowHttpException(string userId, TaskChangeStatusDTO changeDTO)
        {
            WorkspaceTask taskMock = null;
            var taskAssign = AssignOnTaskDTO;

            SetupTaskGetByKeyAsync(taskMock);
            Func<Task> act = () => _taskService.ChangeTaskStatusAsync(changeDTO, userId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCaseSource("ChangeStatus")]
        public async Task ChangeStatus_WithMailing_ReturnTaskComplete(string userId, TaskChangeStatusDTO changeDTO)
        {
            var taskMock = GetWorkspaceTask;
            var userMock = GetUser;
            var workspaceMock = GetWorkspace;
            var statusHistoryMock = StatusHistories[1];

            SetupChangeInfo(userId, userMock, taskMock, Emails, workspaceMock, GetClientUrl);
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
        [TestCaseSource("ChangeTaskInfo")]
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
        [TestCaseSource("ChangeTaskInfo")]
        public async Task ChangeTaskInfo_NoPermission_ThrowHttpException(string userId, TaskChangeInfoDTO changeInfoDTO)
        {
            var taskMock = GetWorkspaceTask;
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
        [TestCaseSource("ChangeTaskInfo")]
        public async Task ChangeTaskInfo_WithMailing_ReturnTaskComplete(string userId, TaskChangeInfoDTO changeInfoDTO)
        {
            var taskMock = GetWorkspaceTask;
            var userMock = GetUser;
            var workspaceMock = GetWorkspace;
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

            SetupChangeInfo(userId, userMock, taskMock, Emails, workspaceMock, GetClientUrl);
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
        [TestCaseSource("CreateTask")]
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
        [TestCaseSource("CreateTask")]
        public async Task CreateTask_UserNotMember_ThrowHttpException(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = GetWorkspace;
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
        [TestCaseSource("CreateTask")]
        public async Task CreateTask_TransactionFailed_ThrowHttpException(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = GetWorkspace;
            var userWorkspaceMock = GetUserWorkspace;
            var taskMock = GetWorkspaceTask;
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
        [TestCaseSource("CreateTask")]
        public async Task CreateTask_TransactionCompleted_ReturnCompletedTask(string userId, TaskCreateDTO taskCreate)
        {
            var workspaceMock = GetWorkspace;
            var userWorkspaceMock = GetUserWorkspace;
            var taskMock = GetWorkspaceTask;

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
        }*/

        [Test]
        [TestCase("2")]
        public async Task GetTaskAttachments_ContentTypeNotFound_ThrowHttpExeption(int taskId)
        {
            var listAttachments = TaskTestData.GetWorkspaceListTaskAttachments().Take(1).ToList();
            var attachmentsListDTO = TaskTestData.GetAttachmentInfoDTOs().Take(1).ToList();
            attachmentsListDTO[0].Name = "name";

            SetupTaskAttachmentGetListById(listAttachments);
            _mapperMock.SetupMap(listAttachments[0], attachmentsListDTO[0]);

            Func<Task> act = () => _taskService.GetTaskAttachmentsAsync(taskId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.CannotGetFileContentType);
        }

        [Test]
        [TestCase("2")]
        public async Task GetTaskAttachments_ThereIsAccessAndExistingData_ReturnTaskAttachmentInfoDTO(int taskId)
        {
            var listAttachments = TaskTestData.GetWorkspaceListTaskAttachments().Take(1).ToList();
            var attachmentsListDTO = TaskTestData.GetAttachmentInfoDTOs().Take(1).ToList();

            SetupTaskAttachmentGetListById(listAttachments);
            _mapperMock.SetupMap(listAttachments[0], attachmentsListDTO[0]);

            var result = await _taskService.GetTaskAttachmentsAsync(taskId);

            result.Should()
                .NotBeNull();
            result.Should()
                .BeEquivalentTo(attachmentsListDTO);
        }

        [Test]
        [TestCase("2")]
        public async Task GetTaskAttachment_AttachmnetsNotFound_ThrowHttpExeption(int attachmentId)
        {
            WorkspaceTaskAttachment attachment = null;

            SetupGetFirstBySpec(attachment);

            Func<Task> act = () => _taskService.GetTaskAttachmentAsync(attachmentId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.AttachmentNotFound);
        }

        [Test]
        [TestCase("2")]
        public async Task GetTaskAttachment_AttachmnetsExist_ReturnDownloadFile(int attachmentId)
        {
            var attachment = TaskTestData.GetWorkspaceTaskAttachment();
            var file = SetupGetDownloadFile();

            SetupGetFirstBySpec(attachment);
            SetupGetFileAsync(file);

            var result = await _taskService.GetTaskAttachmentAsync(attachmentId);

            result.Should()
                .NotBeNull();
            result.Should()
                .BeEquivalentTo(file);
        }

        [Test]
        [TestCase("1")]
        public async Task DeleteTaskAttachment_AttachmentsNotFound_ThrowHttpExeption(int attachmentId)
        {
            WorkspaceTaskAttachment attachment = null;

            SetupGetFirstBySpec(attachment);

            Func<Task> act = () => _taskService.DeleteTaskAttachmentAsync(attachmentId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.AttachmentNotFound);
        }

        [Test]
        [TestCase("1")]
        public async Task DeleteTaskAttachment_AttachmentsExist_ReturnTaskComplate(int attachmentId)
        {
            var attachment = TaskTestData.GetWorkspaceTaskAttachment();

            SetupGetFirstBySpec(attachment);
            SetupDeleteFileAsync(attachment.AttachmentPath);
            SetupAttachmentDeleteAsync(attachment);
            SetupAttachmentSaveChangesAsync();

            var result = Task.Run(() =>
                _taskService.DeleteTaskAttachmentAsync(attachmentId)
            );
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task SendTaskAttachments_AttachmentsMaxCount_ReturnHttpExeption()
        {
            var attachmentsList = TaskTestData.GetWorkspaceListTaskAttachments();
            var taskAttachmentsDTO = TaskTestData.GetTaskAttachmentDTO();
            var maxCount = 2;

            SetupTaskGetListBySpec(attachmentsList);
            SetupAttachmentOptions(maxCount);

            Func<Task> act = () => _taskService.SendTaskAttachmentsAsync(taskAttachmentsDTO);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage($"You have exceeded limit of {maxCount} attachments");
        }

        [Test]
        public async Task SendTaskAttachments_AttachmentsSend_ReturnTaskAttachmentDTO()
        {
            var taskAttachmentsDTO = TaskTestData.GetTaskAttachmentDTO();
            var file = taskAttachmentsDTO.Attachment;
            var folderPath = "file";
            var path = "path";
            var attachmentExpected = TaskTestData.GetAttachmentExpected();
            var maxCount = 1;

            SetupAddFileAsync(folderPath, file.FileName, path);
            SetupAddAttachmentAsync(It.IsAny<WorkspaceTaskAttachment>());
            SetupAttachmentSaveChangesAsync();
            SetupAttachmentOptions(maxCount);
            _mapperMock.SetupMap(It.IsAny<WorkspaceTaskAttachment>(), attachmentExpected);

            var res = await _taskService.SendTaskAttachmentsAsync(taskAttachmentsDTO);
            res.Should().NotBeNull();
            res.Should().BeEquivalentTo(attachmentExpected);        
        }

        [Test]
        [TestCase("2")]
        public async Task GetTaskAttachmentPreview_AttachmnetNotFound_ThrowHttpExeption(int attachmentId)
        {
            WorkspaceTaskAttachment attachment = null;

            SetupGetFirstBySpec(attachment);

            Func<Task> act = () => _taskService.GetTaskAttachmentPreviewAsync(attachmentId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.AttachmentNotFound);
        }

        //[Test]
        //[TestCase("2")]
        //public async Task GetTaskAttachmentPreview_AttachmnetNotPreview_ThrowHttpExeption(int attachmentId)
        //{
        //    var attachment = TaskTestData.GetWorkspaceTaskAttachment();

        //    SetupGetFirstBySpec(attachment);
        //    _imageSettingsOptionsMock(attachment.AttachmentPath)

        //    Func<Task> act = () => _taskService.GetTaskAttachmentPreviewAsync(attachmentId);
        //    await act.Should()
        //        .ThrowAsync<HttpException>()
        //        .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
        //        .WithMessage(ErrorMessages.NotPreview);
        //}

        //[Test]
        //[TestCase("2")]
        //public async Task GetTaskAttachmentPreview_ThereIsAccessAndExistingData_ReturnDownloadFile(int attachmentId)
        //{
        //    var attachment = TaskTestData.GetWorkspaceTaskAttachment();
        //    //var file = TaskTestData.

        //    //SetupGetFirstBySpec(attachment);
        //    //SetupGetFileAsync();

        //    Func<Task> act = () => _taskService.GetTaskAttachmentPreviewAsync(attachmentId);
        //    await act.Should()
        //        .ThrowAsync<HttpException>()
        //        .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
        //        .WithMessage(ErrorMessages.NotPreview);
        //}

        [Test]
        [TestCase("1")]
        public async Task ChangeMemberRole_TaskNotFound_ReturnHttpExeption(string userId)
        {
            var changeRoleDTO = TaskTestData.GetChengeRoleDTO();
            WorkspaceTask task = null;

            SetupTaskGetFirstBySpecAsync(task);

            Func<Task> act = () => _taskService.ChangeMemberRoleAsync(changeRoleDTO, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCase("1")]
        public async Task ChangeMemberRole_UserNotFound_ReturnHttpExeption(string userId)
        {
            var changeRoleDTO = TaskTestData.GetChengeRoleDTO();
            var task = TaskTestData.GetWorkspaceTask();
            UserTask userTask = null;

            SetupTaskGetFirstBySpecAsync(task);
            SetupGetUserTaskFirstBySpec(userTask);

            Func<Task> act = () => _taskService.ChangeMemberRoleAsync(changeRoleDTO, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.UserNotFound);
        }

        [Test]
        [TestCase("3")]
        public async Task ChangeMemberRole_UserNotPermission_ReturnHttpExeption(string userId)
        {
            var changeRoleDTO = TaskTestData.GetChengeRoleDTO();
            var task = TaskTestData.GetWorkspaceTask();
            var userTask = TaskTestData.GetUserTask();
            var userWorkspace = TaskTestData.GetUserWorkspace();

            SetupTaskGetFirstBySpecAsync(task);
            SetupGetUserTaskFirstBySpec(userTask);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspace);

            Func<Task> act = () => _taskService.ChangeMemberRoleAsync(changeRoleDTO, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermission);
        }

        [Test]
        [TestCase("1")]
        public async Task ChangeMemberRole_ThereIsAccessAndExistingData_ReturnTaskComplated(string userId)
        {
            var changeRoleDTO = TaskTestData.GetChengeRoleDTO();
            var task = TaskTestData.GetWorkspaceTask();
                task.TaskCreatorId = userId;
            var userTask = TaskTestData.GetUserTask();
            var userWorkspace = TaskTestData.GetUserWorkspace();

            SetupTaskGetFirstBySpecAsync(task);
            SetupGetUserTaskFirstBySpec(userTask);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspace);
            SetupMetricsDecrement();
            SetupMetricsIncrement();
            SetupUserTaskSaveChangesAsync();

            var result = Task.Run(() =>
                _taskService.ChangeMemberRoleAsync(changeRoleDTO, userId)
            );
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase(2, 2, "2")]
        public async Task DeleteTask_TaskNotFound_ReturnHttpExeption(int workspaceId, int taskId, string userId)
        {
            WorkspaceTask workspaceTask = null;

            SetupTaskGetByKeyAsync(workspaceTask);

            Func<Task> act = () => _taskService.DeleteTaskAsync(workspaceId, taskId, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCase(2, 2, "1")]
        public async Task DeleteTask_UserNotPermission_ReturnHttpExeption(int workspaceId, int taskId, string userId)
        {
            var workspaceTask = TaskTestData.GetWorkspaceTask();

            SetupTaskGetByKeyAsync(workspaceTask);

            Func<Task> act = () => _taskService.DeleteTaskAsync(workspaceId, taskId, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermission);
        }

        [Test]
        [TestCase(2, 2, "2")]
        public async Task DeleteTask_ThereIsAccessAndExistingData_ReturnTaskComplated(int workspaceId, int taskId, string userId)
        {
            var workspaceTask = TaskTestData.GetWorkspaceTask();
            var statusHistoriesList = TaskTestData.GetStatusHistoriesList();
            var commentList = TaskTestData.GetCommentList();
            var attachmentList = TaskTestData.GetWorkspaceListTaskAttachments();
            var userTaskList = TaskTestData.GetListUserTask();


            SetupTaskGetByKeyAsync(workspaceTask);
            SetupMetricsDecrement();
            SetupStatusHistoryGetListBySpecAsync(statusHistoriesList);
            SetupStatusHistoryDeleteRange();
            SetupCommentGetListBySpecAsync(commentList);
            SetupCommentDeleteRange();
            SetupAttachmentGetListBySpecAsync(attachmentList);
            SetupAttachmentDeleteRange();
            SetupUserTaskGetListBySpecAsync(userTaskList);
            SetupUserTaskDeleteRange();
            SetupMetricsIncrement();
            SetupWorkspaceTaskDeleteAsync();
            SetupWorkspaceTaskSaveChangesAsync();

            var result = Task.Run(() =>
                _taskService.DeleteTaskAsync(workspaceId, taskId, userId)
            );
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        [TestCase(2, 2, "1", "2")]
        public async Task DisjoinTask_TaskNotFound_ReturnHttpExeption(int workspaceId, int taskId, string disUserId, string userId)
        {
            WorkspaceTask task = null;

            SetupTaskGetFirstBySpecAsync(task);

            Func<Task> act = () => _taskService.DisjoinTaskAsync(workspaceId, taskId, disUserId, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.TaskNotFound);
        }

        [Test]
        [TestCase(2, 2, "1", "2")]
        public async Task DisjoinTask_UserTaskNotFound_ReturnHttpExeption(int workspaceId, int taskId, string disUserId, string userId)
        {
            var task = TaskTestData.GetWorkspaceTask();
            UserTask userTask = null;

            SetupTaskGetFirstBySpecAsync(task);
            SetupUserTaskGetFirstBySpecAsync(userTask);

            Func<Task> act = () => _taskService.DisjoinTaskAsync(workspaceId, taskId, disUserId, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.NotFound)
                .WithMessage(ErrorMessages.UserNotFound);
        }

        [Test]
        [TestCase(2, 2, "1", "3")]
        public async Task DisjoinTask_UserNotPermission_ReturnHttpExeption(int workspaceId, int taskId, string disUserId, string userId)
        {
            var task = TaskTestData.GetWorkspaceTask();
            var userTask = TaskTestData.GetUserTask();
            var userWorkspace = TaskTestData.GetUserWorkspace();

            SetupTaskGetFirstBySpecAsync(task);
            SetupUserTaskGetFirstBySpecAsync(userTask);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspace);

            Func<Task> act = () => _taskService.DisjoinTaskAsync(workspaceId, taskId, disUserId, userId);
            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermission);
        }

        [Test]
        [TestCase(2, 2, "1", "2")]
        public async Task DisjoinTask_ThereIsAccessAndExistingData_ReturnTaskComplated(int workspaceId, int taskId, string disUserId, string userId)
        {
            var task = TaskTestData.GetWorkspaceTask();
            var userTask = TaskTestData.GetUserTask();
            var userWorkspace = TaskTestData.GetUserWorkspace();

            SetupTaskGetFirstBySpecAsync(task);
            SetupUserTaskGetFirstBySpecAsync(userTask);
            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspace);
            SetupMetricsDecrement();
            SetupUserTaskDeleteAsync();
            SetupUserTaskSaveChangesAsync();

            var result = Task.Run(() =>
                _taskService.DisjoinTaskAsync(workspaceId, taskId, disUserId, userId)
            );
            result.Wait();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
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
                .Returns(Task.FromResult(statuses))
                .Verifiable();
        }

        protected void SetupRolesGetAllAsync(IEnumerable<UserRoleTag> roles)
        {
            _userRoleTagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(roles))
                .Verifiable();
        }

        protected void SetupHistoryGetBySpecAsync(IEnumerable<StatusHistory> histories)
        {
            _statusHistoryRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<StatusHistory>>()))
                .ReturnsAsync(histories);
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
                .Returns(Task.FromResult(task))
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
                .Returns(Task.FromResult(1))
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
                .Returns(Task.CompletedTask)
                .Verifiable();
        }
    }
}
