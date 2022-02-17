using Ardalis.Specification;

namespace Provis.Core.Entities.SprintEntity
{
    public class Sptints
    {
        internal class SprintInfo : Specification<Sprint>
        {
            public SprintInfo(int workspaceId)
            {
                Query
                    .Where(x => x.WorkspaceId == workspaceId);
            }
        }
    }
}
