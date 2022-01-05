﻿using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.workspaceDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task ChangeTaskStatusAsync(ChangeTaskStatusDTO cangeTaskStatus);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId);
        Task<List<TaskStatusDTO>> GetTaskStatuses();
        Task<List<WorkerRoleDTO>> GetWorkerRoles();
    }
}
