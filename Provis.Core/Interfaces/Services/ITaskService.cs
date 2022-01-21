using Provis.Core.ApiModels;
using Provis.Core.DTO.TaskDTO;using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task ChangeTaskStatusAsync(TaskChangeStatusDTO cangeTaskStatus, string userId);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId);
        Task<List<TaskStatusDTO>> GetTaskStatuses();
        Task ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId);
        Task JoinTaskAsync(TaskAssignDTO taskAssignDTO, string userId);
        Task<List<TaskRoleDTO>> GetWorkerRoles();
        Task<List<TaskStatusHistoryDTO>> GetStatusHistories(int taskId);
        Task<TaskInfoDTO> GetTaskInfoAsync(int taskId);
        Task<List<TaskAttachmentInfoDTO>> GetTaskAttachmentsAsync(int taskId);
        Task<DownloadFile> GetTaskAttachmentAsync(int attachmentId);
        Task DeleteTaskAttachmentAsync(int attachmentId);
        Task<TaskAttachmentInfoDTO> SendTaskAttachmentsAsync(TaskAttachmentsDTO taskAttachmentsDTO);
        Task<DownloadFile> GetTaskAttachmentPreviewAsync(int attachmentId);
        Task ChangeMemberRoleAsync(TaskChangeRoleDTO changeRoleDTO, string userId);
        Task DisjoinTaskAsync(int workspaceId, int taskId, string disUserId, string userId);
    }
}
