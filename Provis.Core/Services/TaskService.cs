using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

namespace Provis.Core.Services
{
    public class TaskService : ITaskService
    {
        protected readonly UserManager<User> _userManager;
        public readonly IRepository<User> _userRepository;
        public readonly IRepository<Workspace> _workspaceRepository;
        public readonly IRepository<Entities.Task> _taskRepository;
        public readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IMapper _mapper;

        public TaskService(IRepository<User> user,
            IRepository<Entities.Task> task,
            IRepository<Workspace> workspace,
            IRepository<UserTask> userTask,
            UserManager<User> userManager,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _userRepository = user;
            _taskRepository = task;
            _workspaceRepository = workspace;
            _userTaskRepository = userTask;
            _mapper = mapper;
        }

        public async Task<List<TaskDTO>> GetUserTasksAsync(string userId, int workspaceId)
        {
            IEnumerable<Entities.Task> userTasks = null;
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

        public async Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            _ = user ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "User with Id not exist");

            var workspaceRec = await _workspaceRepository.GetByKeyAsync(taskCreateDTO.WorkspaceId);

            _ = workspaceRec ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Workspace with Id not found");

            var task = new Entities.Task();

            _mapper.Map(taskCreateDTO, task);

            task.DateOfCreate = DateTime.UtcNow;
            task.TaskCreatorId = user.Id;

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            if (taskCreateDTO.AssignedUsers.Count !=0)
            {
                List<UserTask> userTasks = new List<UserTask>(); 
                foreach (var item in taskCreateDTO.AssignedUsers)
                {
                    if (true)
                    {

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
