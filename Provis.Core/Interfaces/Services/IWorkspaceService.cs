using Provis.Core.DTO.workspaceDTO;
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
        Task SendInviteAsync(InviteUserDTO inviteUser, string userId);
        Task<ChangeRoleDTO> ChangeUserRoleAsync(string userId, ChangeRoleDTO userChangeRole);
        Task UpdateWorkspaceAsync(WorkspaceUpdateDTO workspaceDTO, string userId);
        Task<WorkspaceInfoDTO> GetWorkspaceInfoAsync(int id, string userEmail);
        Task<List<WorkspaceInviteInfoDTO>> GetWorkspaceActiveInvitesAsync(int id, string userId);
        Task<List<WorkspaceMemberDTO>> GetWorkspaceMembersAsync(int workspaceId);
        Task DeleteFromWorkspaceAsync(int workspId, string userId);
        Task CancelInviteAsync(int id, int workspaceId, string userId);
        Task<List<WorkspaceRolesDTO>> GetAllowedRoles();
    }
}

