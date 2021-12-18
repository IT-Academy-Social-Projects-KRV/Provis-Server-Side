using Provis.Core.DTO.workspaceDTO;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IInviteUserService
    {
        Task SendInviteAsync(InviteUserDTO inviteUser, string userId);
    }
}
