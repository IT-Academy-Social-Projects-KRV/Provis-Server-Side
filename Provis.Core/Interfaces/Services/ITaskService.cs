using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.workspaceDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetUserTasksAsync(string userId, int workspaceId);
        Task ChangeTaskStatus(ChangeTaskStatusDTO cangeTaskStatus);
    }
}
