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
using Provis.Core.Helpers.Mails;

namespace Provis.Core.Services
{
    public class TaskService : ITaskService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<WorkspaceTask> _taskRepository;
        protected readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<StatusHistory> _statusHistoryRepository;
        protected readonly IRepository<Status> _taskStatusRepository;
        protected readonly IRepository<UserRoleTag> _workerRoleRepository;
        protected readonly IRepository<WorkspaceTaskAttachment> _taskAttachmentRepository;
        protected readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IOptions<TaskAttachmentSettings> _attachmentSettings;

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
            IFileService fileService,
            IOptions<TaskAttachmentSettings> attachmentSettings,
            UserManager<User> userManager
            )
        {
            _userManager = userManager;
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
            _attachmentSettings = attachmentSettings;
            _fileService = fileService;
        }

        public async Task ChangeTaskStatusAsync(TaskChangeStatusDTO changeTaskStatus, string userId)
        {
            var task = await _taskRepository.GetByKeyAsync(changeTaskStatus.TaskId);
            task.TaskNullChecking();

            var statusHistory = new StatusHistory
            {
                DateOfChange = DateTime.UtcNow,
                StatusId = changeTaskStatus.StatusId,
                TaskId = task.Id,
                UserId = userId
            };

            await _statusHistoryRepository.AddAsync(statusHistory);

            task.StatusId = changeTaskStatus.StatusId;
            await _taskRepository.UpdateAsync(task);

            await _taskRepository.SaveChangesAsync();
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

            task.DateOfCreate = DateTime.UtcNow;
            task.TaskCreatorId = userId;
            task.StatusHistories.Add(new StatusHistory()
            {
                StatusId = taskCreateDTO.StatusId,
                DateOfChange = DateTime.UtcNow,
                UserId = userId
            });

            _mapper.Map(taskCreateDTO, task);

            using (var transaction = await _taskRepository.BeginTransactionAsync())
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
                                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                                    "This user has already assigned");
                            }
                            userTasks.Add(new UserTask
                            {
                                TaskId = task.Id,
                                UserId = item.UserId,
                                UserRoleTagId = item.RoleTagId
                            });
                        }
                        await _userTaskRepository.AddRangeAsync(userTasks);
                        await _userTaskRepository.SaveChangesAsync();
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                        "Failed");
                }
            }
            await Task.CompletedTask;
        }

        public async Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                var specification = new UserTasks.UserTaskList(userId, workspaceId);
                var selection = await _userTaskRepository.GetListBySpecAsync(specification);

                var result = selection
                    .GroupBy(x => x.Item1)
                    .ToDictionary(k => k.Key,
                        v => v.Select(x => _mapper.Map<TaskDTO>(x.Item2))
                    .ToList());

                return new TaskGroupByStatusDTO()
                {
                    UserId = userId,
                    Tasks = result
                };
            }
            else
            {
                var specification = new WorkspaceTasks.UnassignedTaskList(workspaceId);
                var selection = await _taskRepository.GetListBySpecAsync(specification);

                var result = selection
                    .GroupBy(x => x.Item1)
                    .ToDictionary(k => k.Key,
                        v => v.Select(x => _mapper.Map<TaskDTO>(x.Item2))
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
            var taskSpecification = new WorkspaceTasks.TaskById(taskAssign.Id);
            var task = await _taskRepository.GetFirstBySpecAsync(taskSpecification);

            var workspaceSpecification = new Workspaces.WorkspaceById(taskAssign.WorkspaceId);
            var worksp = await _workspaceRepository.GetFirstBySpecAsync(workspaceSpecification);

            if (task.TaskCreatorId != userId && taskAssign.AssignedUsers.Single().UserId != userId)
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                        "Only creator of the task can assign another users");
            }

            List<UserTask> userTasks = new();
            foreach (var item in taskAssign.AssignedUsers)
            {
                if (userTasks.Exists(x => x.UserId == item.UserId))
                {
                    throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                        "This user has already assigned");
                }
                if (!worksp.UserWorkspaces.Exists(c => c.UserId == item.UserId))
                {
                    throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                        "This user doesn't member of current workspace");
                }
                if (task.UserTasks.Exists(x => x.UserId == item.UserId && !x.IsUserDeleted))
                {
                    throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                        "This user alredy in this task");
                }

                userTasks.Add(new UserTask
                {
                    TaskId = task.Id,
                    UserId = item.UserId,
                    UserRoleTagId = item.RoleTagId
                });
            }
            await _userTaskRepository.AddRangeAsync(userTasks);
            await _userTaskRepository.SaveChangesAsync();

        }

        public async Task ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId)
        {
            var workspaceTask = await _taskRepository.GetByKeyAsync(taskChangeInfoDTO.Id);
            workspaceTask.TaskNullChecking();

            if (workspaceTask.TaskCreatorId != userId)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You are not the creator of the task");
            }

            _mapper.Map(taskChangeInfoDTO, workspaceTask);

            await _taskRepository.UpdateAsync(workspaceTask);

            await _taskRepository.SaveChangesAsync();
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

            _ = task ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Task with Id not found");

            var taskToRerutn = _mapper.Map<TaskInfoDTO>(task);

            return taskToRerutn;
        }

        public async Task<List<TaskAttachmentInfoDTO>> GetTaskAttachmentsAsync(int taskId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachments(taskId);
            var listAttachments = await _taskAttachmentRepository.GetListBySpecAsync(specification);

            return listAttachments.Select(x => _mapper.Map<TaskAttachmentInfoDTO>(x)).ToList();
        }
        
        public async Task<DownloadFile> GetTaskAttachmentAsync(int attachmentId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachmentInfo(attachmentId);
            var attachment = await _taskAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Attachment not found");

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }
        
        public async Task DeleteTaskAttachmentAsync(int attachmentId)
        {
            var specification = new WorkspaceTaskAttachments.TaskAttachmentInfo(attachmentId);
            var attachment = await _taskAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Attachment not found");

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
                throw new HttpException(System.Net.HttpStatusCode.BadRequest,
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

            return _mapper.Map<TaskAttachmentInfoDTO>(workspaceTaskAttachment);
        }
    }
}
