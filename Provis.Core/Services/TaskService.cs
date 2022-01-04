using AutoMapper;
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

namespace Provis.Core.Services
{
    public class TaskService : ITaskService
    {
        public readonly IRepository<User> _userRepository;
        public readonly IRepository<Workspace> _workspaceRepository;
        public readonly IRepository<Entities.Task> _taskRepository;
        protected readonly IMapper _mapper;

        public TaskService(IRepository<User> user,
            IRepository<Entities.Task> task,
            IRepository<Workspace> workspace,
            IMapper mapper
            )
        {
            _userRepository = user;
            _taskRepository = task;
            _workspaceRepository = workspace;
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

        public async System.Threading.Tasks.Task ChangeTaskStatusAsync(ChangeTaskStatusDTO changeTaskStatus)
        {
            var task = await _taskRepository.GetByKeyAsync(changeTaskStatus.TaskId);

            _ = task ?? throw new HttpException(System.Net.HttpStatusCode.NotFound, "Task not found");

            //Add logic to add record to statusHistory table

            task.StatusId = changeTaskStatus.StatusId;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

    }
}
