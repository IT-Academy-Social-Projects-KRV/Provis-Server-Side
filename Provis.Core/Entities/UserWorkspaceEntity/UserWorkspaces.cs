using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspaces
    {
        internal class WorkspaceList : Specification<UserWorkspace>
        {
            public WorkspaceList(string userId)
            {
                Query
                    .Where(y => y.UserId == userId)
                    .Include(x => x.Workspace)
                    .OrderBy(x => x.RoleId)
                    .ThenBy(x => x.Workspace.Name);
            }
        }

        internal class WorkspaceMember : Specification<UserWorkspace>
        {
            public WorkspaceMember(string userId, int workspaceId)
            {
                Query
                    .Where(p => p.UserId == userId &&
                    p.WorkspaceId == workspaceId);
            }
        }

        internal class WorkspaceMemberList : Specification<UserWorkspace>
        {
            public WorkspaceMemberList(int workspaceId)
            {
                Query
                    .Where(u => u.WorkspaceId == workspaceId)
                    .Include(u => u.User)
                    .OrderBy(o => o.RoleId);
            }
        }

        internal class WorkspaceInfo : Specification<UserWorkspace>
        {
            public WorkspaceInfo(string userId, int workspaceId)
            {
                Query
                    .Where(x => x.WorkspaceId == workspaceId
                           && x.UserId == userId)
                    .Include(x => x.Workspace);
            }
        }
    }
}
