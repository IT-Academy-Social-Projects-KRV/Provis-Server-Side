using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;
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
                    .Include(x => x.Users)
                    .Where(x => x.WorkspaceId == workspaceId
                    && !x.Users.Any())
                    .ToListAsync();
            }
            else
            {
                userTasks = await _taskRepository.Query()
                   .Include(x => x.Users)
                   .Where(x => x.WorkspaceId == workspaceId
                   && x.Users.Any())
                   .ToListAsync();
            }

            var mapTask = _mapper.Map<List<TaskDTO>>(userTasks);
            return mapTask;
        }
    }
}
