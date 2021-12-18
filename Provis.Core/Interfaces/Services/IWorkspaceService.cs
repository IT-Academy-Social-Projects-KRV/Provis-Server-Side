using Provis.Core.DTO.workspaceDTO;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IWorkspaceService 
    {
        Task CreateWorkspace(WorkspaceCreateDTO workspaceDTO, string userid);

        Task SendInviteAsync(InviteUserDTO inviteUser, string userId);
    }
}
