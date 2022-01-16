using Ardalis.Specification;

namespace Provis.Core.Entities.WorkspaceEntity
{
    public class Workspaces
    {
        internal class WorkspaceById : Specification<Workspace>
        {
            public WorkspaceById(int workspaceId)
            {
                Query
                    .Where(p => p.Id == workspaceId)
                    .Include(p => p.UserWorkspaces);
            }
        }
    }
}
