using Ardalis.Specification;
using System;
using System.Linq;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTasks
    {
        internal class UnassignedTaskList : Specification<WorkspaceTask, Tuple<int, WorkspaceTask, int>>
        {
            public UnassignedTaskList(int workspaceId)
            {
                Query
                    .Select(x => new Tuple<int, WorkspaceTask, int>(
                        x.StatusId,
                        x,
                        x.Comments.Count))
                    .Where(x => x.WorkspaceId == workspaceId && (!x.UserTasks.Any() || x.UserTasks.All(y => y.IsUserDeleted == true))
                    .OrderBy(x => x.StatusId);
            }
        }

        internal class TaskById : Specification<WorkspaceTask>
        {
            public TaskById(int taskId)
            {
                Query
                    .Where(p => p.Id == taskId)
                    .Include(p => p.UserTasks
                        .Where(x => !x.IsUserDeleted))
                        .ThenInclude(x => x.User);
            }
        }
    }
}
