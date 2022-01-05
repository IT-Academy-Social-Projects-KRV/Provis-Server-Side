using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskEntity = Provis.Core.Entities.Task;

namespace Provis.Core.Services
{
    public class TaskService : ITaskService
    {
        protected readonly UserManager<User> _userManager;
        public readonly IRepository<User> _userRepository;
        public readonly IRepository<Workspace> _workspaceRepository;
        public readonly IRepository<TaskEntity> _taskRepository;
        public readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IMapper _mapper;
        private readonly IRepository<StatusHistory> _statusHistoryRepository;

        public TaskService(IRepository<User> user,
            IRepository<TaskEntity> task,
            IRepository<Workspace> workspace,
            IMapper mapper,
            IRepository<StatusHistory> statusHistoryRepository
            IRepository<UserTask> userTask,
            UserManager<User> userManager
            )
        {
            _userManager = userManager;
            _userRepository = user;
            _taskRepository = task;
            _workspaceRepository = workspace;
            _userTaskRepository = userTask;
            _mapper = mapper;
            _statusHistoryRepository = statusHistoryRepository;
        }

        public async Task<List<TaskDTO>> GetUserTasksAsync(string userId, int workspaceId)
        {
            IEnumerable<TaskEntity> userTasks = null;
            if (String.IsNullOrEmpty(userId))
            {
                userTasks = await _taskRepository.Query()
                    .Include(x => x.UserTasks)
                    .Include(x => x.Status)
                    .Where(x => x.WorkspaceId == workspaceId &&
                        !x.UserTasks.Any())
                    .ToListAsync();
            }
            else
            {
                userTasks = await _taskRepository.Query()
                   .Include(x => x.UserTasks)
                   .Include(x => x.Status)
                   .Where(x => x.WorkspaceId == workspaceId &&
                        x.UserTasks.Any())
                   .ToListAsync();
            }

            var mapTask = _mapper.Map<List<TaskDTO>>(userTasks);
            return mapTask;
        }

        public async System.Threading.Tasks.Task ChangeTaskStatusAsync(ChangeTaskStatusDTO changeTaskStatus)
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

            var workspaceRec = await _workspaceRepository.GetByKeyAsync(taskCreateDTO.WorkspaceId);

            _ = workspaceRec ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Workspace with Id not found");

            var task = new TaskEntity();

            task.DateOfCreate = DateTime.UtcNow;
            task.TaskCreatorId = user.Id;

            _mapper.Map(taskCreateDTO, task);

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
            await Task.CompletedTask;
        }
    }
}
