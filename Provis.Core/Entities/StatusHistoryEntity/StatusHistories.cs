using Ardalis.Specification;

namespace Provis.Core.Entities.StatusHistoryEntity
{
    public class StatusHistories
    {
        internal class StatusHistoresList : Specification<StatusHistory>
        {
            public StatusHistoresList(int taskId)
            {
                Query
                    .Where(x => x.TaskId == taskId)
                    .Include(x => x.Status)
                    .Include(x => x.User);
            }
        }
    }
}
