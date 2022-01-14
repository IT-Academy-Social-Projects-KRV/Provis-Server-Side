﻿using Provis.Core.DTO.TaskDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task ChangeTaskStatusAsync(TaskChangeStatusDTO cangeTaskStatus);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId);
        Task<List<TaskStatusDTO>> GetTaskStatuses();
        Task JoinTaskAsync(TaskAssignDTO taskAssignDTO, string userId);
        Task<List<TaskRoleDTO>> GetWorkerRoles();
    }
}
