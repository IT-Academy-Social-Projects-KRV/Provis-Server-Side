using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Provis.Core.ApiModels;
using Provis.Core.Helpers;
using Microsoft.Extensions.Options;
using Provis.Core.Roles;
using Provis.Core.Helpers.Mails;
using Provis.Core.Helpers.Mails.ViewModels;
using Provis.Core.Statuses;
using App.Metrics;
using Provis.Core.Metrics;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using Provis.Core.Resources;
using Ardalis.Specification;

namespace Provis.Core.Services
{
    public class TaskService : ITaskService
    {
        protected readonly UserManager<User> _userManager;
        private readonly IOptions<ImageSettings> _imageSettings;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<WorkspaceTask> _taskRepository;
        protected readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<StatusHistory> _statusHistoryRepository;
        protected readonly IRepository<Status> _taskStatusRepository;
        protected readonly IRepository<UserRoleTag> _workerRoleRepository;
        protected readonly IRepository<WorkspaceTaskAttachment> _taskAttachmentRepository;
        protected readonly IRepository<Comment> _commentRepository;
        protected readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IOptions<TaskAttachmentSettings> _attachmentSettings;
        private readonly IOptions<ClientUrl> _clientUrl;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IMetrics _metrics;

        public TaskService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Workspace> workspace,
            IRepository<Status> taskStatusRepository,
            IRepository<UserWorkspace> userWorkspace,
            IMapper mapper,
            IRepository<StatusHistory> statusHistoryRepository,
            IRepository<UserTask> userTask,
            IRepository<UserRoleTag> workerRoleRepository,
            IRepository<WorkspaceTaskAttachment> taskAttachmentRepository,
            IRepository<Comment> commentRepository,
            IFileService fileService,
            IOptions<TaskAttachmentSettings> attachmentSettings,
            UserManager<User> userManager,
            IOptions<ClientUrl> options,
            IEmailSenderService emailSenderService,
            IOptions<ImageSettings> imageSettings,
            IMetrics metrics
            )
        {
            _userManager = userManager;
            _imageSettings = imageSettings;
            _userRepository = user;
            _taskRepository = task;
            _userWorkspaceRepository = userWorkspace;
            _workspaceRepository = workspace;
            _userTaskRepository = userTask;
            _mapper = mapper;
            _statusHistoryRepository = statusHistoryRepository;
            _workerRoleRepository = workerRoleRepository;
            _taskStatusRepository = taskStatusRepository;
            _taskAttachmentRepository = taskAttachmentRepository;
            _commentRepository = commentRepository;
            _attachmentSettings = attachmentSettings;
            _fileService = fileService;
            _clientUrl = options;
            _emailSenderService = emailSenderService;
            _metrics = metrics;
        }

        public async Task ChangeTaskStatusAsync(TaskChangeStatusDTO changeTaskStatus, string userId)
        {
            var task = await _taskRepository.GetByKeyAsync(changeTaskStatus.TaskId);
            task.TaskNullChecking();

            if(task.StatusId != changeTaskStatus.StatusId)
            {
                var user = await _userRepository.GetByKeyAsync(userId);

                var workspace = await _workspaceRepository.GetByKeyAsync(changeTaskStatus.WorkspaceId);

                var assignedUserEmails = await _userTaskRepository.GetListBySpecAsync(
                    new UserTasks.TaskAssignedUserEmailList(changeTaskStatus.TaskId));

                if(assignedUserEmails.Count() > 1)
                {
                    await Task.Factory.StartNew(async () =>
                    {
                        await _emailSenderService.SendManyMailsAsync(new MailingRequest<TaskChangeStatus>()
                        {
                            Emails = assignedUserEmails,
                            Subject = $"Changed status of {task.Name} task",
                            Body = "TaskStatusChange",
                            ViewModel = new TaskChangeStatus()
                            {
                                Uri = _clientUrl.Value.ApplicationUrl,
                                WhoChangedUserName = user.Name,
                                FromStatus = (TaskStatuses)task.StatusId,
                                ToStatus = (TaskStatuses)changeTaskStatus.StatusId,
                                TaskName = task.Name,
                                WorkspaceName = workspace.Name
                            }
                        });
                    });
                }

                var statusHistory = new StatusHistory
                {
                    DateOfChange = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                    StatusId = changeTaskStatus.StatusId,
                    TaskId = task.Id,
                    UserId = userId
                };

            _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskCountByStatus,
               MetricTagsConstructor.TaskCountByStatus(changeTaskStatus.WorkspaceId, changeTaskStatus.StatusId));

            _metrics.Measure.Counter.Decrement(WorkspaceMetrics.TaskCountByStatus,
                MetricTagsConstructor.TaskCountByStatus(task.WorkspaceId, task.StatusId));

            await _statusHistoryRepository.AddAsync(statusHistory);

                task.StatusId = changeTaskStatus.StatusId;
                await _taskRepository.UpdateAsync(task);

                await _taskRepository.SaveChangesAsync();
            }
        }

        public async Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(taskCreateDTO.WorkspaceId);
            workspace.WorkspaceNullChecking();

            foreach (var item in taskCreateDTO.AssignedUsers)
            {
                var specification = new UserWorkspaces.WorkspaceMember(item.UserId, workspace.Id);
                var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);
                userWorkspace.UserWorkspaceNullChecking();
            }

            var task = new WorkspaceTask();

            task.DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
            task.TaskCreatorId = userId;
            task.StatusHistories.Add(new StatusHistory()
            {
                StatusId = taskCreateDTO.StatusId,
                DateOfChange = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                UserId = userId
            });

            _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskCountByStatus,
                        MetricTagsConstructor.TaskCountByStatus(taskCreateDTO.WorkspaceId, taskCreateDTO.StatusId));

            _mapper.Map(taskCreateDTO, task);

            using (var transaction = await _taskRepository.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead))
            {
                try
                {
                    await _taskRepository.AddAsync(task);
                    await _taskRepository.SaveChangesAsync();

                    if (taskCreateDTO.AssignedUsers.Count != 0)
                    {
                        List<UserTask> userTasks = new List<UserTask>();
                        foreach (var item in taskCreateDTO.AssignedUsers)
                        {
                            if (userTasks.Exists(x => x.UserId == item.UserId))
                            {
                                throw new HttpException(HttpStatusCode.Forbidden,
                                    ErrorMessages.UserAlreadyAssigned);
                            }

                            userTasks.Add(new UserTask
                            {
                                TaskId = task.Id,
                                UserId = item.UserId,
                                UserRoleTagId = item.RoleTagId
                            });

                            _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskRolesCountByWorkspace,
                                MetricTagsConstructor.TaskRolesCountByWorkspace(taskCreateDTO.WorkspaceId, item.RoleTagId));
                        }
                        await _userTaskRepository.AddRangeAsync(userTasks);
                        await _userTaskRepository.SaveChangesAsync();
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new HttpException(HttpStatusCode.Forbidden,
                        ErrorMessages.TransactionFailed);
                }
            }
            await Task.CompletedTask;
        }

        public async Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId, int? sprintId)
        {
            var workspase = await _workspaceRepository.GetByKeyAsync(workspaceId);

            if (!String.IsNullOrEmpty(userId))
            {
                UserTasks.UserTaskList specification;

                if(workspase.isUseSprints)
                    specification = new UserTasks.UserTaskList(userId, workspaceId, sprintId);
                else
                    specification = new UserTasks.UserTaskList(userId, workspaceId);

                var selection = await _userTaskRepository.GetListBySpecAsync(specification);

                var result = selection
                    .GroupBy(x => x.Item1)
                    .ToDictionary(k => k.Key,
                        v => v.Select(x => _mapper.Map<TaskDTO>(x))
                    .ToList());

                return new TaskGroupByStatusDTO()
                {
                    UserId = userId,
                    Tasks = result
                };
            }
            else
            {
                WorkspaceTasks.UnassignedTaskList specification;

                if(workspase.isUseSprints)
                    specification = new WorkspaceTasks.UnassignedTaskList(workspaceId, sprintId);
                else
                    specification = new WorkspaceTasks.UnassignedTaskList(workspaceId);

                var selection = await _taskRepository.GetListBySpecAsync(specification);

                var result = selection
                    .GroupBy(x => x.Item1)
                    .ToDictionary(k => k.Key,
                        v => v.Select(x => _mapper.Map<TaskDTO>(x))
                    .ToList());

                return new TaskGroupByStatusDTO()
                {
                    UserId = null,
                    Tasks = result
                };
            }
        }

        public async Task<List<TaskStatusDTO>> GetTaskStatuses()
        {
            var result = await _taskStatusRepository.GetAllAsync();

            return result.Select(x => _mapper.Map<TaskStatusDTO>(x)).ToList();
        }

        public async Task<List<TaskRoleDTO>> GetWorkerRoles()
        {
            var result = await _workerRoleRepository.GetAllAsync();

            return result.Select(x => _mapper.Map<TaskRoleDTO>(x)).ToList();
        }

        public async Task JoinTaskAsync(TaskAssignDTO taskAssign, string userId)
        {
            var task = await _taskRepository.GetByKeyAsync(taskAssign.Id);
            ExtensionMethods.TaskNullChecking(task);

            if (task.TaskCreatorId != userId && taskAssign.AssignedUser.UserId != userId)
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermissionAssignUser);
            }

            var userWorkspaceSpecification = new UserWorkspaces
                .WorkspaceMember(taskAssign.AssignedUser.UserId, taskAssign.WorkspaceId);

            if(!await _userWorkspaceRepository.AnyBySpecAsync(userWorkspaceSpecification))
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.UserNotMember);
            }

            var userTaskSpecifiction = new UserTasks
                .AssignedMember(taskAssign.Id, taskAssign.AssignedUser.UserId);

            if(await _userTaskRepository.AnyBySpecAsync(userTaskSpecifiction))
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.UserAlreadyAssigned);
            }

            _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskRolesCountByWorkspace,
                    MetricTagsConstructor.TaskRolesCountByWorkspace(taskAssign.WorkspaceId, taskAssign.AssignedUser.RoleTagId));

            var userToAssign = _mapper.Map<UserTask>(taskAssign);
            await _userTaskRepository.AddAsync(userToAssign);
            await _userTaskRepository.SaveChangesAsync();
        }

        public async Task ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId)
        {
            var workspaceTask = await _taskRepository.GetByKeyAsync(taskChangeInfoDTO.Id);
            workspaceTask.TaskNullChecking();

            var worspaceMemberSpecefication = new UserWorkspaces
                .WorkspaceMember(userId, taskChangeInfoDTO.WorkspaceId);

            if (!(workspaceTask.TaskCreatorId == userId ||
                await _userWorkspaceRepository
                .AllBySpecAsync(worspaceMemberSpecefication, x => x.RoleId == (int)WorkSpaceRoles.OwnerId)))
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }

            if (workspaceTask.Name != taskChangeInfoDTO.Name
                || workspaceTask.Description != taskChangeInfoDTO.Description
                || workspaceTask.DateOfEnd != taskChangeInfoDTO.Deadline
                || workspaceTask.StoryPoints != taskChangeInfoDTO.StoryPoints)
            {
                var user = await _userRepository.GetByKeyAsync(userId);

                var workspace = await _workspaceRepository.GetByKeyAsync(taskChangeInfoDTO.WorkspaceId);

                var assignedUserEmails = await _userTaskRepository.GetListBySpecAsync(
                    new UserTasks.TaskAssignedUserEmailList(taskChangeInfoDTO.Id));

                if (assignedUserEmails.Count() > 1)
                {
                    await Task.Factory.StartNew(async () =>
                    {
                        await _emailSenderService.SendManyMailsAsync(new MailingRequest<TaskEdited>()
                        {
                            Emails = assignedUserEmails,
                            Subject = $"Task {workspaceTask.Name} was edited",
                            Body = "TaskChange",
                            ViewModel = new TaskEdited()
                            {
                                Uri = _clientUrl.Value.ApplicationUrl,
                                WhoEditUserName = user.Name,
                                TaskChangeInfo = taskChangeInfoDTO,
                                WorkspaceName = workspace.Name
                            }
                        });
                    });
                }

                _mapper.Map(taskChangeInfoDTO, workspaceTask);

                await _taskRepository.UpdateAsync(workspaceTask);

                await _taskRepository.SaveChangesAsync();
            }
        }

        public async Task<List<TaskStatusHistoryDTO>> GetStatusHistories(int taskId)
        {
            var specification = new StatusHistories.StatusHistoresList(taskId);
            var selection = await _statusHistoryRepository.GetListBySpecAsync(specification);

            var statusHustoryList = _mapper.Map<List<TaskStatusHistoryDTO>>(selection);

            return statusHustoryList;
        }

        public async Task<TaskInfoDTO> GetTaskInfoAsync(int taskId)
        {
            var specification = new WorkspaceTasks.TaskById(taskId);
            var task = await _taskRepository.GetFirstBySpecAsync(specification);

            _ = task ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.TaskNotFound);

            var taskToRerutn = _mapper.Map<TaskInfoDTO>(task);

            return taskToRerutn;
        }

        public async Task<List<TaskAttachmentInfoDTO>> GetTaskAttachmentsAsync(int taskId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachments(taskId);
            var listAttachments = await _taskAttachmentRepository.GetListBySpecAsync(specification);

            var listToReturn = listAttachments.Select(x => _mapper.Map<TaskAttachmentInfoDTO>(x)).ToList();
            var provider = new FileExtensionContentTypeProvider();
            foreach (var item in listToReturn)
            {
                if(provider.TryGetContentType(item.Name, out string contentType))
                {
                    item.ContentType = contentType;
                }
                else
                {
                    throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.CannotGetFileContentType);
                }
            }
            return listToReturn;
        }

        public async Task<DownloadFile> GetTaskAttachmentAsync(int attachmentId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachmentInfo(attachmentId);
            var attachment = await _taskAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.AttachmentNotFound);

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }

        public async Task DeleteTaskAttachmentAsync(int attachmentId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachmentInfo(attachmentId);
            var attachment = await _taskAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.AttachmentNotFound);

            if (attachment.AttachmentPath != null)
            {
                await _fileService.DeleteFileAsync(attachment.AttachmentPath);
            }

            await _taskAttachmentRepository.DeleteAsync(attachment);
            await _taskAttachmentRepository.SaveChangesAsync();
        }

        public async Task<TaskAttachmentInfoDTO> SendTaskAttachmentsAsync(TaskAttachmentsDTO taskAttachmentsDTO)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachments(taskAttachmentsDTO.TaskId);
            var result = await _taskAttachmentRepository.GetListBySpecAsync(specification);

            var listAttachmentsAlready = result.ToList();

            if (listAttachmentsAlready.Count == _attachmentSettings.Value.MaxCount)
                throw new HttpException(HttpStatusCode.BadRequest,
                    $"You have exceeded limit of {_attachmentSettings.Value.MaxCount} attachments");

            var file = taskAttachmentsDTO.Attachment;

            string newPath = await _fileService.AddFileAsync(file.OpenReadStream(),
                _attachmentSettings.Value.Path, file.FileName);

            WorkspaceTaskAttachment workspaceTaskAttachment = new()
            {
                AttachmentPath = newPath,
                TaskId = taskAttachmentsDTO.TaskId
            };

            await _taskAttachmentRepository.AddAsync(workspaceTaskAttachment);

            await _taskAttachmentRepository.SaveChangesAsync();

            var res = _mapper.Map<TaskAttachmentInfoDTO>(workspaceTaskAttachment);
            res.ContentType = taskAttachmentsDTO.Attachment.ContentType;

            return res;
        }

        public async Task<DownloadFile> GetTaskAttachmentPreviewAsync(int attachmentId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachmentInfo(attachmentId);
            var attachment = await _taskAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.AttachmentNotFound);

            var provider = new FileExtensionContentTypeProvider();

            if (provider.TryGetContentType(attachment.AttachmentPath, out string contentType) &&
                !contentType.StartsWith(_imageSettings.Value.Type))
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.NotPreview);
            }

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }

        public async Task ChangeMemberRoleAsync(TaskChangeRoleDTO changeRoleDTO, string userId)
        {
            var taskSpecification = new WorkspaceTasks
                    .TaskById(changeRoleDTO.TaskId);
            var task = await _taskRepository
                    .GetFirstBySpecAsync(taskSpecification);

            _ = task ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.TaskNotFound);

            var userTaskSpecification = new UserTasks
                    .AssignedMember(changeRoleDTO.TaskId, changeRoleDTO.UserId);
            var userTaskMember = await _userTaskRepository
                    .GetFirstBySpecAsync(userTaskSpecification);

            _ = userTaskMember ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.UserNotFound);

            var userSpecification = new UserWorkspaces
                    .WorkspaceMember(userId, changeRoleDTO.WorkspaceId);
            var user = await _userWorkspaceRepository
                    .GetFirstBySpecAsync(userSpecification);

            if (task.TaskCreatorId == userId ||
                (WorkSpaceRoles)user.RoleId == WorkSpaceRoles.OwnerId)
            {
                _metrics.Measure.Counter.Decrement(WorkspaceMetrics.TaskRolesCountByWorkspace,
                    MetricTagsConstructor.TaskRolesCountByWorkspace(changeRoleDTO.WorkspaceId, userTaskMember.UserRoleTagId));

                userTaskMember.UserRoleTagId = changeRoleDTO.RoleId;

                _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskRolesCountByWorkspace,
                    MetricTagsConstructor.TaskRolesCountByWorkspace(changeRoleDTO.WorkspaceId, changeRoleDTO.RoleId));

                await _userTaskRepository.SaveChangesAsync();
            }
            else
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }
        }

        public async Task DisjoinTaskAsync(int workspaceId, int taskId, string disUserId, string userId)
        {
            var taskSpecification = new WorkspaceTasks
                    .TaskById(taskId);
            var task = await _taskRepository
                    .GetFirstBySpecAsync(taskSpecification);

            _ = task ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.TaskNotFound);

            var userTaskSpecification = new UserTasks
                    .AssignedMember(taskId, disUserId);
            var userTaskMember = await _userTaskRepository
                    .GetFirstBySpecAsync(userTaskSpecification);

            _ = userTaskMember ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.UserNotFound);

            var userSpecification = new UserWorkspaces
                    .WorkspaceMember(userId, workspaceId);
            var user = await _userWorkspaceRepository
                    .GetFirstBySpecAsync(userSpecification);

            if (task.TaskCreatorId == userId ||
                (WorkSpaceRoles)user.RoleId == WorkSpaceRoles.OwnerId)
            {
                _metrics.Measure.Counter.Decrement(WorkspaceMetrics.TaskRolesCountByWorkspace,
                    MetricTagsConstructor.TaskRolesCountByWorkspace(workspaceId, userTaskMember.UserRoleTagId));

                await _userTaskRepository.DeleteAsync(userTaskMember);
                await _userTaskRepository.SaveChangesAsync();
            }
            else
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }
        }

        public async Task DeleteTaskAsync(int workspaceId, int taskId, string userId)
        {
            var workspaceTask = await _taskRepository.GetByKeyAsync(taskId);

            _ = workspaceTask ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.TaskNotFound);

            var worspaceMemberSpecefication = new UserWorkspaces.WorkspaceMember(userId, workspaceId);

            if (!(workspaceTask.TaskCreatorId == userId ||
                await _userWorkspaceRepository
                .AllBySpecAsync(worspaceMemberSpecefication, x => x.RoleId == (int) WorkSpaceRoles.OwnerId)))
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }

            _metrics.Measure.Counter.Decrement(WorkspaceMetrics.TaskCountByStatus,
                MetricTagsConstructor.TaskCountByStatus(workspaceTask.WorkspaceId, workspaceTask.StatusId));

            var specificationStatusHistories = new StatusHistories.StatusHistoresList(taskId);
            var statusHistoriesList = await _statusHistoryRepository.GetListBySpecAsync(specificationStatusHistories);

            await _statusHistoryRepository.DeleteRangeAsync(statusHistoriesList);

            var specificationComments = new Comments.CommentTask(taskId);
            var commentsList = await _commentRepository.GetListBySpecAsync(specificationComments);

            await _commentRepository.DeleteRangeAsync(commentsList);

            var specificationAttachments = new WorkspaceTaskAttachments.TaskAttachments(taskId);
            var taskAttachmentsList = await _taskAttachmentRepository.GetListBySpecAsync(specificationAttachments);

            await _taskAttachmentRepository.DeleteRangeAsync(taskAttachmentsList);

            var specificationUserTask = new UserTasks.TaskUserList(taskId);
            var userTaskList = await _userTaskRepository.GetListBySpecAsync(specificationUserTask);

            await _userTaskRepository.DeleteRangeAsync(userTaskList);

            foreach (var item in userTaskList)
            {
                _metrics.Measure.Counter.Increment(WorkspaceMetrics.TaskRolesCountByWorkspace,
                    MetricTagsConstructor.TaskRolesCountByWorkspace(workspaceTask.WorkspaceId, item.UserRoleTagId));
            }

            await _taskRepository.DeleteAsync(workspaceTask);
            await _taskRepository.SaveChangesAsync();
        }
    }
}
