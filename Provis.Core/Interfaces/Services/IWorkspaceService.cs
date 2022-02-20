using Provis.Core.DTO.WorkspaceDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IWorkspaceService
    {
        Task CreateWorkspaceAsync(WorkspaceCreateDTO workspaceDTO, string userid);
        Task DenyInviteAsync(int id, string userid);
        Task AcceptInviteAsync(int id, string userid);
        Task<List<WorkspaceInfoDTO>> GetWorkspaceListAsync(string userid);
        Task SendInviteAsync(WorkspaceInviteUserDTO inviteUser, string userId);
        Task<WorkspaceChangeRoleDTO> ChangeUserRoleAsync(string userId, WorkspaceChangeRoleDTO userChangeRole);
        Task UpdateWorkspaceAsync(WorkspaceUpdateDTO workspaceDTO, string userId);
        Task<WorkspaceInfoDTO> GetWorkspaceInfoAsync(int id, string userId);
        Task<WorkspaceDescriptionDTO> GetWorkspaceDescriptionAsync(int workspaceId);
        Task<List<WorkspaceInviteInfoDTO>> GetWorkspaceActiveInvitesAsync(int id, string userId);
        Task<List<WorkspaceMemberDTO>> GetWorkspaceMembersAsync(int workspaceId);
        Task DeleteFromWorkspaceAsync(int workspId, string userId);
        Task CancelInviteAsync(int id, int workspaceId, string userId);
        Task<List<WorkspaceRoleDTO>> GetAllowedRoles();
        Task<List<WorkspaceDetailMemberDTO>> GetDetailMemberAsyns(int workspaceId);
        Task SetUsingSprintsAsync(int workspaceId, bool isUseSptints);
    }
}

