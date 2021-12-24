using Microsoft.AspNetCore.Authorization;

namespace Provis.WebApi.Policy
{
    public class WorkspaceRolesRequirement: IAuthorizationRequirement
    {
        public int[] WorkspaceRolesId { get; private set; }

        public WorkspaceRolesRequirement(int[] workspaceRolesId) { WorkspaceRolesId = workspaceRolesId; }
    }
}
