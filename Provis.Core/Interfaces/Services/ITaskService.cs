using Provis.Core.ApiModels;
using Provis.Core.DTO.TaskDTO;using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITaskService
    {
        Task<TaskChangeStatusDTO> ChangeTaskStatusAsync(TaskChangeStatusDTO cangeTaskStatus, string userId);
        Task CreateTaskAsync(TaskCreateDTO taskCreateDTO, string userId);
        Task<TaskGroupByStatusDTO> GetTasks(string userId, int workspaceId, int? sprintId);
        Task<List<TaskStatusDTO>> GetTaskStatuses();
        Task<TaskInfoDTO> ChangeTaskInfoAsync(TaskChangeInfoDTO taskChangeInfoDTO, string userId);
        Task JoinTaskAsync(TaskAssignDTO taskAssignDTO, string userId);
        Task<List<TaskRoleDTO>> GetWorkerRoles();
        Task<List<TaskStatusHistoryDTO>> GetStatusHistories(int taskId);
        Task<TaskInfoDTO> GetTaskInfoAsync(int taskId);
        Task<List<TaskAttachmentInfoDTO>> GetTaskAttachmentsAsync(int taskId);
        Task<DownloadFile> GetTaskAttachmentAsync(int attachmentId);
        Task DeleteTaskAttachmentAsync(int attachmentId);
        Task<TaskAttachmentInfoDTO> SendTaskAttachmentsAsync(TaskAttachmentsDTO taskAttachmentsDTO);
        Task<DownloadFile> GetTaskAttachmentPreviewAsync(int attachmentId);
        Task<TaskChangeRoleDTO> ChangeMemberRoleAsync(TaskChangeRoleDTO changeRoleDTO, string userId);
        Task DisjoinTaskAsync(int workspaceId, int taskId, string disUserId, string userId);
        Task DeleteTaskAsync(int workspaceId, int taskId, string userId);
        Task MoveTaskToSprint(int taskId, int workspaceId, int? sprintId);
    }
}
