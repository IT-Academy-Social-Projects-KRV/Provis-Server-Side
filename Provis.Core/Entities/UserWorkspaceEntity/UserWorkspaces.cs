using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspaces
    {
        internal class GetWorkspaceList: Specification<UserWorkspace>
        {
            public GetWorkspaceList(string userId)
            {
                Query
                    .Where(y => y.UserId == userId)
                    .Include(x => x.Workspace)
                    .Include(x => x.Role)
                    .OrderBy(x => x.RoleId)
                    .ThenBy(x => x.Workspace.Name);
            }
        }
    }
}
