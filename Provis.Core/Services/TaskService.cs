using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        protected readonly IMapper _mapper;


        public TaskService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Workspace> workspace,
            IRepository<Status> taskStatusRepository,
            IRepository<UserWorkspace> userWorkspace,
            IMapper mapper,
            IRepository<StatusHistory> statusHistoryRepository,
            IRepository<UserTask> userTask,
            IRepository<UserRoleTag> workerRoleRepository,
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
        }

        public async Task ChangeTaskStatusAsync(TaskChangeStatusDTO changeTaskStatus)
        {
            var task = await _taskRepository.GetByKeyAsync(changeTaskStatus.TaskId);

            _ = task ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Task not found");

            var statusHistory = new StatusHistory
            {
                DateOfChange = DateTime.UtcNow,
                StatusId = changeTaskStatus.StatusId,
                TaskId = task.Id
            };

            await _statusHistoryRepository.AddAsync(statusHistory);

            task.StatusId = changeTaskStatus.StatusId;
            await _taskRepository.UpdateAsync(task);

            await _taskRepository.SaveChangesAsync();
        }

        public async Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            _ = user ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "User with Id not exist");

            var workspace = await _workspaceRepository.GetByKeyAsync(taskCreateDTO.WorkspaceId);

            _ = workspace ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Workspace with Id not found");

            foreach (var item in taskCreateDTO.AssignedUsers)
            {
                var specification = new UserWorkspaces.WorkspaceMember(item.UserId, workspace.Id);
                var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

                _ = userWorkspace ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "User in workspace not found");
            }

            var task = new WorkspaceTask();

            task.DateOfCreate = DateTime.UtcNow;
            task.TaskCreatorId = user.Id;

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

        public async Task ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId)
        {
            var workspaceTask = await _taskRepository.GetByKeyAsync(taskChangeInfoDTO.Id);

            _ = workspaceTask ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Task with Id not found");

            if(workspaceTask.TaskCreatorId != userId)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You are not the creator of the task");
            }

            _mapper.Map(taskChangeInfoDTO, workspaceTask);

            await _taskRepository.UpdateAsync(workspaceTask);

            await _taskRepository.SaveChangesAsync();
        }

        public async Task<TaskInfoDTO> GetTaskInfoAsync(int taskId)
        {
            var task = await _taskRepository.GetByKeyAsync(taskId);

            _ = task ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Task with Id not found");

            TaskInfoDTO taskInfoDTO = new TaskInfoDTO();

            _mapper.Map(task, taskInfoDTO);

            var specification = new UserTasks.TaskUserList(taskId);

            var taskUsers = await _userTaskRepository.GetListBySpecAsync(specification);

            List<TaskAssignedUsersDTO> userList = new List<TaskAssignedUsersDTO>();

            foreach (var item in taskUsers)
            {
                userList.Add(new TaskAssignedUsersDTO { 
                    UserId = item.UserId, 
                    UserName = item.User.UserName, 
                    RoleTagId = item.UserRoleTagId });
            }

            taskInfoDTO.AssignedUsers = userList;

            return taskInfoDTO;
        }
    }
}
