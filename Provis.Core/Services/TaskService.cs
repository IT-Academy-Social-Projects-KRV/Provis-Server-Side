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
        protected readonly IMapper _mapper;

        public TaskService(IRepository<User> user,
            IRepository<Entities.Task> task,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IMapper mapper
            )
        {
            _userManager = userManager;
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

        public async Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            //_ = user ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
            //"User with Id not exist");

            //var workspaceRec = await _workspaceRepository.GetByKeyAsync(workspaceUpdateDTO.WorkspaceId);

            //_ = workspaceRec ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
            //"Workspace with with Id not found");
            await Task.CompletedTask;
        }
    }
}
