using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.workspaceDTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskEntity = Provis.Core.Entities.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task<List<TaskLastDTO>> GetUserTasksAsync(string userId, int workspaceId);
        Task ChangeTaskStatusAsync(ChangeTaskStatusDTO cangeTaskStatus);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId);
        Task GetStatuses(string userId, int workspaceId);
    }
}
