using Microsoft.EntityFrameworkCore;
using Provis.Core.Interfaces;
using Provis.Core.Interfaces.Repositories;
using System.Linq;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspaces
    {
        internal class GetWorkspaceList : IQuery<UserWorkspace>
        {
            public IQueryable<UserWorkspace> Query { get; }
            public GetWorkspaceList(string userId, IRepository<UserWorkspace> repository)
            {
                Query = repository.Query()
                    .Where(y => y.UserId == userId)
                    .Include(x => x.Workspace)
                    .Include(x => x.Role)
                    .OrderBy(x => x.RoleId)
                    .ThenBy(x => x.Workspace.Name);
            }
        }

    }
}
