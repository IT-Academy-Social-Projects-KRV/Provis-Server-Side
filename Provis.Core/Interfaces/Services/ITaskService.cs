using Provis.Core.ApiModels;
using Provis.Core.DTO.TaskDTO;using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task ChangeTaskStatusAsync(TaskChangeStatusDTO cangeTaskStatus);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId);
        Task<List<TaskStatusDTO>> GetTaskStatuses();
        Task ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId);
        Task<List<TaskRoleDTO>> GetWorkerRoles();
        Task<List<TaskAttachmentInfoDTO>> GetTaskAttachmentsAsync(int taskId);
        Task<DownloadFile> GetTaskAttachmentAsync(int attachmentId);
        Task DeleteTaskAttachmentAsync(int attachmentId);
        Task SendTaskAttachmentsAsync(TaskAttachmentsDTO taskAttachmentsDTO);
    }
}
